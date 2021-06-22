using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContractReaderV2.Concrete;
using Code7248.word_reader;
using System.IO;
using System.Text.RegularExpressions;
using static ContractReaderV2.Concrete.Enum.GlobalEnum;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

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

        public List<Contract> ParseWordDocument()
        {
            var extractor = new TextExtractor(_documentPath);
            var docText = extractor.ExtractText();
            File.WriteAllText(_tempDocumentPath, docText);
            ParseDocument();
            return _lineList;

        }

        public List<Contract> ParsePdfDocument()
        {
            if (File.Exists(_documentPath))
            {
                var doc = PDDocument.load(_documentPath);
                var textStrip = new PDFTextStripper();
                var strPdfText = textStrip.getText(doc);
                doc.close();
                File.WriteAllText(_tempDocumentPath, strPdfText);
            }
            ParseDocument();
            return _lineList;
        }

        public void ParseDocument()
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
                CycleThrough(textList, lineCounter);
            }
            File.Delete(_tempDocumentPath);
        }

        public void CycleThrough(List<string> lines, int lineAmount)
        {
            int lineCount = 0;
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

                if (!string.IsNullOrEmpty(newSentence))
                {
                    contract.Data = newSentence;
                }
                else
                {
                    contract.Data = lineData;
                }

                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Contractor;
                if (!string.IsNullOrEmpty(contract.Data.Trim()))
                {
                    _lineList.Add(contract);
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
