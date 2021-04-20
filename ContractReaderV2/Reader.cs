using System;
using Code7248.word_reader;
using ContractReaderV2.Concrete;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ContractReaderV2.Concrete.Enum;
using static ContractReaderV2.Concrete.Enum.GlobalEnum;

namespace ContractReaderV2
{
    public class Reader
    {
        private readonly string _documentPath;
        private readonly string _tempDocumentPath;
        private readonly List<Contract> _lineList;
        private const string ContractorWill = "contractor will";
        private const string ContractorShall = "contractor shall";
        private const string ContractorsShall = "contractors shall";
        private const string GovWill = "the government will";
        private const string GovShall = "the government shall";
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
            var extractor = new TextExtractor(_documentPath);
            var docText = extractor.ExtractText();
            File.WriteAllText(_tempDocumentPath, docText);
            ParseTempDocument(keywords, replacements, DocumentType.Doc);
            return _lineList;

        }

        public List<Contract> ParsePdfDocument(List<string> keywords, List<string> replacements)
        {
            var wordList2 = new List<string>();
            if (File.Exists(_documentPath))
            {
                var pdfReader = new PdfReader(_documentPath);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    var words = new List<string>();
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    var currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    words.AddRange(currentText.Split('\n').ToList());
                    for (int j = 0, len = words.Count; j < len; j++)
                    {
                        wordList2.Add( Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(words[j]))));
                    }
                    //text.Append(currentText);
                }
                pdfReader.Close();
                File.AppendAllLines(_tempDocumentPath, wordList2);
            }
            ParseTempDocument(keywords, replacements, DocumentType.Pdf);
            return _lineList;
        }

        public void ParseTempDocument(List<string> keywords, List<string> replacements, GlobalEnum.DocumentType docType)
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
                FirstPass(textList, 0, lineCounter,keywords, replacements, docType);
            }
            File.Delete(_tempDocumentPath);
            
        }

        public void FirstPass(List<string> lines, int lineCount, int lineAmount, List<string> keywords, List<string> replacements, GlobalEnum.DocumentType docType, LineType lineType = LineType.Generic)
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

                //Grab the section number if this line contains one
                var section = GetDocumentSection2(lineData);

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

                if (firstLine != "")
                {
                    //This is a second line of a contractor hit.
                    string[] sentences = Regex.Split(lineData, @"(?<=[\.!\?])\s+");
                    foreach (var sentence in sentences)
                    {
                        lineData = firstLine + "" + sentence;
                        break;
                    }
                    var contract = new Contract();
                    foreach (var replacement in replacements)
                    {
                        if (lineData.ToLower().Contains(replacement))
                        {
                            lineData = Regex.Replace(lineData, replacement, _parseHitReplace,
                                RegexOptions.IgnoreCase);
                        }
                    }
                    contract.Data = lineData;

                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Contractor;
                    _lineList.Add(contract);
                    lineType = LineType.Contractor;

                    firstLine = "";
                }
                else
                {
                    foreach (var keyword in keywords)
                    {
                        if (lines[lineCount].ToLower().Contains(keyword.ToLower()))
                        {
                            var i = lineData.ToLower().IndexOf(keyword.ToLower());
                            if (i > 0)
                            {
                                lineData = lineData.Remove(0, i);
                            }

                            if (docType == DocumentType.Doc)
                            {
                                string[] sentences = Regex.Split(lineData, @"(?<=[\.!\?])\s+");
                                foreach (var sentence in sentences)
                                {
                                    if (sentence.ToLower().Contains(ContractorShall))
                                    {
                                        var contract = new Contract();
                                        var newSentence = string.Empty;
                                        foreach (var replacement in replacements)
                                        {
                                            if (sentence.ToLower().Contains(replacement))
                                            {
                                                newSentence = Regex.Replace(sentence, replacement, _parseHitReplace,
                                                    RegexOptions.IgnoreCase);
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(newSentence))
                                        {
                                            contract.Data = newSentence;
                                        }
                                        else
                                        {
                                            contract.Data = sentence;
                                        }

                                        contract.DocumentSection = _lastSectionId;
                                        contract.DataType = LineType.Contractor;
                                        _lineList.Add(contract);
                                        lineType = LineType.Contractor;
                                    }
                                }
                            }
                            else if (docType == DocumentType.Pdf)
                            {
                                if (lineData.ToLower().Contains(ContractorShall))
                                {
                                    string[] sentences = Regex.Split(lineData, @"(?<=[\.!\?])\s+");
                                    if (sentences.Length == 1)
                                    {
                                        //sentence doesn't end on this line.
                                        firstLine = lineData;
                                    }
                                    else
                                    {
                                        //sentence does end on this line.
                                        var contract = new Contract();
                                        foreach (var replacement in replacements)
                                        {
                                            if (lineData.ToLower().Contains(replacement))
                                            {
                                                lineData = Regex.Replace(lineData, replacement, _parseHitReplace,
                                                    RegexOptions.IgnoreCase);
                                            }
                                        }

                                        contract.Data = lineData;

                                        contract.DocumentSection = _lastSectionId;
                                        contract.DataType = LineType.Contractor;
                                        _lineList.Add(contract);
                                        lineType = LineType.Contractor;
                                    }
                                }
                            }
                        }
                    }
                }

                lineCount = lineCount + 1;








                ////Look for keywords
                //if (lines[lineCount].ToLower().Contains(GovShall))
                //{
                //    contract.Data = lineData;
                //    contract.DocumentSection = _lastSectionId;
                //    contract.DataType = LineType.Government;
                //    _lineList.Add(contract);
                //    lineCount = lineCount + 1;
                //    lineType = LineType.Government;
                //}
                //else if (lines[lineCount].ToLower().Contains(GovWill))
                //{
                //    contract.Data = lineData;
                //    contract.DocumentSection = _lastSectionId;
                //    contract.DataType = LineType.Government;
                //    _lineList.Add(contract);
                //    lineCount = lineCount + 1;
                //    lineType = LineType.Contractor;
                //}
                //else if (lines[lineCount].ToLower().Contains(ContractorShall))
                //{
                //    lineData = Regex.Replace(lineData, ContractorShall, _parseHitReplace, RegexOptions.IgnoreCase);
                //    contract.Data = lineData;
                //    contract.DocumentSection = _lastSectionId;
                //    contract.DataType = LineType.Contractor;
                //    _lineList.Add(contract);
                //    lineCount = lineCount + 1;
                //    lineType = LineType.Contractor;
                //}
                //else if (lines[lineCount].ToLower().Contains(ContractorsShall))
                //{
                //    lineData = Regex.Replace(lineData, ContractorsShall, _parseHitReplace, RegexOptions.IgnoreCase);
                //    contract.Data = lineData;
                //    contract.DocumentSection = _lastSectionId;
                //    contract.DataType = LineType.Contractor;
                //    _lineList.Add(contract);
                //    lineCount = lineCount + 1;
                //    lineType = LineType.Contractor;
                //}
                //else if (lines[lineCount].ToLower().Contains(ContractorWill))
                //{
                //    lineData = Regex.Replace(lineData, ContractorWill, _parseHitReplace, RegexOptions.IgnoreCase);
                //    contract.Data = lineData;
                //    contract.DocumentSection = _lastSectionId;
                //    contract.DataType = LineType.Contractor;
                //    _lineList.Add(contract);
                //    lineCount = lineCount + 1;
                //    lineType = LineType.Contractor;
                //}
                //else
                //{
                //    contract.Data = lineData;
                //    contract.DocumentSection = _lastSectionId;
                //    contract.DataType = LineType.Generic;
                //    lineCount = lineCount + 1;
                //    lineType = LineType.Generic;
                //}
            }
        }

        public string GetDocumentSection(string line)
        {
            string section = string.Empty;
         
            for(var k = 0; k < line.Length;k++)
            {
                var isNumber = int.TryParse(line[k].ToString(), out _);
                if(k == 0 && !isNumber)
                {
                    return string.Empty;
                }
                else
                {
                    if(isNumber || line[k].ToString()=="." )
                    {
                        section += line[k].ToString();
                    }
                    else
                    {
                        return section;
                    }
                }
            }
            return section;
        }

        public string GetDocumentSection2(string line)
        {
            var t = Regex.Match(line, @"^[A-Z0-9][\w-]*(?:\.[\w-]+)*");
            if (t != null && t.Success)
            {
                return t.Value;
            }
            return string.Empty;
        }

    }
}
