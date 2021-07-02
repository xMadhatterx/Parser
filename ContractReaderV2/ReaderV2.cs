using System.Collections.Generic;
using ContractReaderV2.Concrete;
using Code7248.word_reader;
using System.IO;
using System.Text.RegularExpressions;
using static ContractReaderV2.Concrete.Enum.GlobalEnum;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
namespace ContractReaderV2
{
    public class ReaderV2
    {
        private readonly string _documentPath;
        private readonly string _tempDocumentPath;
        private readonly List<Contract> _lineList;
        private string _lastSectionId;

        public ReaderV2(string documentPath, string tempPath)
        {
            if (string.IsNullOrWhiteSpace(documentPath) || string.IsNullOrWhiteSpace(tempPath)) return;
            _documentPath = documentPath;
            _tempDocumentPath = tempPath;
            _lineList = new List<Contract>();
        }

        public List<Contract> ParseWordDocument(List<string> keywords)
        {
            var extractor = new TextExtractor(_documentPath);
            var docText = extractor.ExtractText();
            File.WriteAllText(_tempDocumentPath, docText);
            ParseDocument(keywords);
            return _lineList;
        }

        public List<Contract> ParsePdfDocument(List<string> keywords)
        {
            if (File.Exists(_documentPath))
            {
                var doc = PDDocument.load(_documentPath);
              
                var textStrip = new PDFTextStripper();
                var strPdfText = textStrip.getText(doc);
                doc.close();
                File.WriteAllText(_tempDocumentPath, strPdfText);
            }
            ParseDocument(keywords);
            return _lineList;
        }

        //public void ParsePage()
        //{
        //    using (PdfReader reader = new PdfReader(_documentPath))
        //    {

        //        List<string> page1 = new List<string>();
        //        List<string> page2 = new List<string>();
        //        var strText=PdfTextExtractor.GetTextFromPage(reader, 2);
        //        strText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(strText)));
        //        string[] lines = strText.Split('\n');
        //        foreach(var line in lines)
        //        {
        //            if (!string.IsNullOrWhiteSpace(line))
        //            {
        //                page1.Add(line);
        //            }
        //        }
        //        var headerPage1 = page1[0];
        //        var footerPage1 = page1[page1.Count()-1];

        //        var strText2 = PdfTextExtractor.GetTextFromPage(reader, 5);
        //        strText2 = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(strText2)));
        //        string[] lines2 = strText2.Split('\n');
        //        foreach (var line in lines2)
        //        {
        //            if (!string.IsNullOrWhiteSpace(line))
        //            {
        //                page2.Add(line);
        //            }
        //        }
        //        var headerPage2 = page2[0];
        //        var footerPage2 = page2[page2.Count()-1];

        //    }
        //}

        public void ParseDocument(List<string> keywords)
        {
            var textList = new List<string>();
            var lineCounter = 0;
            using (var reader = new StreamReader(new FileStream(_tempDocumentPath, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    textList.Add(reader.ReadLine());
                    lineCounter++;
                }
                CycleThrough(textList, lineCounter, keywords);
            }
            File.Delete(_tempDocumentPath);
        }

        public void CycleThrough(List<string> lines, int lineAmount, List<string> keywords)
        {
            var lineCount = 0;
            while (true)
            {
                if (lineCount >= lineAmount) return;
                var lineData = lines[lineCount];

                //Remove \t from doc
                if (lineData.StartsWith("\t"))
                {
                    lineData = lineData.Replace("\t", "");
                }

                //Second check to replace any mid line tabs with spaces.
                if (lineData.Contains("\t"))
                {
                    lineData = lineData.Replace("\t", " ");
                }

                //Grab the section number if this line contains one
                var section = GetDocumentSection(lineData);

                //Set current section if we were able to find one.
                if (!string.IsNullOrWhiteSpace(section))
                {
                    _lastSectionId = section;
                }

                //Remove Section from current line
                if (!string.IsNullOrEmpty(section))
                {
                    lineData = lineData.Remove(0, section.Length + 1);
                }

                var contract = new Contract();
                var newSentence = string.Empty;

                contract.Data = !string.IsNullOrEmpty(newSentence) ? newSentence : lineData;

                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Contractor;
                if (!string.IsNullOrEmpty(contract.Data.Trim()))
                {
                    foreach(string keyword in keywords)
                    {
                        if (contract.Data.Contains(keyword))
                        {
                            _lineList.Add(contract);
                            break;
                        }
                    }
                }
                lineCount++;
            }
        }

        public string GetDocumentSection(string line)
        {
            var section = string.Empty;
            var match = false;
            var leadingLetter = false;
            var sectionChecker = new Regex(@"(?m)^\d+(?:\.\d+)*[ \t]+\S.*$");
            var sectionCheckerTrailing = new Regex(@"(?m)^\d+(?:\.\d+)*\S[ \t]+\S.*$");
            var sectionCheckerLeading = new Regex(@"(?m)^\S.\d+(?:\.\d+)*[ \t]+\S.*$");

            if (sectionChecker.IsMatch(line))
            {
                match = true;
            }
            else if (sectionCheckerTrailing.IsMatch(line))
            {
                match = true;
            }
            else if (sectionCheckerLeading.IsMatch(line))
            {
                match = true;
                leadingLetter = true;
            }

            if (!match) return section;
            foreach (var t in line)
            {
                if (leadingLetter)
                {
                    section += t.ToString();
                    leadingLetter = false;
                }
                else
                {
                    if (t.ToString() == " ")
                    {
                        return section;
                    }
                    var isNumber = int.TryParse(t.ToString(), out _);
                    if (isNumber || t.ToString() == ".")
                    {
                        section += t.ToString();
                    }
                }
            }
            return section;
        }

    }
}
