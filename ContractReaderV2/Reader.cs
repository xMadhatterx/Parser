using System;
using Code7248.word_reader;
using ContractReaderV2.Concrete;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ContractReaderV2.Concrete.Enum;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using static ContractReaderV2.Concrete.Enum.GlobalEnum;

namespace ContractReaderV2
{
    public class Reader
    {
        private readonly string _documentPath;
        private readonly string _tempDocumentPath;
        private readonly List<Contract> _lineList;
        private string _lastSectionId;
        private string _parseHitReplace = "My company name";

        public Reader(string documentPath,string tempPath)//,DocumentType documentType)
        {
            if (string.IsNullOrWhiteSpace(documentPath) || string.IsNullOrWhiteSpace(tempPath)) return;
            _documentPath = documentPath;
            _tempDocumentPath = tempPath;
            _lineList = new List<Contract>();
        }

        public List<Contract> ParseWordDocument(List<string> keywords, List<string> replacements)
        {
            try
            {
                var extractor = new TextExtractor(_documentPath);
                var docText = extractor.ExtractText();
                File.WriteAllText(_tempDocumentPath, docText);
                ParseTempDocument(keywords, replacements);
                return _lineList;
            }
            catch
            {
                return null;
            }
        }

        public List<Contract> ParsePdfDocument(List<string> keywords, List<string> replacements)
        {
            //var wordList2 = new List<string>();
            if (File.Exists(_documentPath))
            {
                //Using PDFBox instead of iTextSharp
                var doc = PDDocument.load(_documentPath);
                var textStrip = new PDFTextStripper();
                var strPdfText = textStrip.getText(doc);
                doc.close();
                File.WriteAllText(_tempDocumentPath, strPdfText);

                //var pdfReader = new PdfReader(_documentPath);

                //for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                //{
                //    var words = new List<string>();
                //    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                //    var currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                //    words.AddRange(currentText.Split('\n').ToList());
                //    for (int j = 0, len = words.Count; j < len; j++)
                //    {
                //        wordList2.Add(Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(words[j]))));
                //    }
                //    //text.Append(currentText);
                //}
                //pdfReader.Close();
                //File.AppendAllLines(_tempDocumentPath, wordList2);
            }
            ParseTempDocument(keywords, replacements);
            return _lineList;
        }

        public void ParseTempDocument(List<string> keywords, List<string> replacements)
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
                FirstPass(textList, 0, lineCounter,keywords, replacements);
            }
            File.Delete(_tempDocumentPath);
            
        }

        public void FirstPass(List<string> lines, int lineCount, int lineAmount, List<string> keywords, List<string> replacements, LineType lineType = LineType.Generic)
        {
            var firstLine = string.Empty;

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
                    lineData = lineData.Remove(0, section.Length);
                }

                
                foreach (var keyword in keywords)
                {
                    if (lines[lineCount].ToLower().Contains(keyword.ToLower()))
                    {
                        var i = lineData.ToLower().IndexOf(keyword.ToLower());
                        if (i > 0)
                        {
                            lineData = lineData.Remove(0, i);
                        }

                        //string[] sentences = Regex.Split(lineData, @"(?<=[\.!\?])\s+");
                        //foreach (var sentence in sentences)
                        //{
                        //if (sentence.ToLower().Contains(keyword))
                        if (lineData.ToLower().Contains(keyword))
                        {
                            var contract = new Contract();
                            var newSentence = string.Empty;
                            foreach (var replacement in replacements)
                            {
                                //if (sentence.ToLower().Contains(replacement))
                                if (lineData.ToLower().Contains(replacement))
                                {
                                    //newSentence = Regex.Replace(sentence, replacement, _parseHitReplace,
                                    //    RegexOptions.IgnoreCase);
                                    newSentence = Regex.Replace(lineData, replacement, _parseHitReplace,
                                        RegexOptions.IgnoreCase);
                                }
                            }

                            if (!string.IsNullOrEmpty(newSentence))
                            {
                                contract.Data = newSentence;
                            }
                            else
                            {
                                //contract.Data = sentence;
                                contract.Data = lineData;
                            }

                            contract.DocumentSection = _lastSectionId;
                            contract.DataType = LineType.Contractor;
                            _lineList.Add(contract);
                            lineType = LineType.Contractor;
                        }
                        //}
                    }
                }
                
                lineCount = lineCount + 1;
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
            for (var k = 0; k < line.Length; k++)
            {
                if (leadingLetter)
                {
                    section += line[k].ToString();
                    leadingLetter = false;
                }
                else
                {
                    if (line[k].ToString() == " ")
                    {
                        return section;
                    }
                    var isNumber = int.TryParse(line[k].ToString(), out _);
                    if (isNumber || line[k].ToString() == ".")
                    {
                        section += line[k].ToString();
                    }
                }
            }
            return section;
        }

    }
}
