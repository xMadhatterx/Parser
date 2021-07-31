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
        private readonly List<Contract> _lineList2;
        private string _lastSectionId;

        public ReaderV2(string documentPath, string tempPath)
        {
            if (string.IsNullOrWhiteSpace(documentPath) || string.IsNullOrWhiteSpace(tempPath)) return;
            _documentPath = documentPath;
            _tempDocumentPath = tempPath;
            _lineList = new List<Contract>();
            _lineList2 = new List<Contract>();
        }

        public List<Contract> ParseWordDocument(List<Word> keywords)
        {
            var extractor = new TextExtractor(_documentPath);
            var docText = extractor.ExtractText();
            File.WriteAllText(_tempDocumentPath, docText);
            ParseDocument(keywords);
            //return _lineList;
            return _lineList2;
        }

        public List<Contract> ParsePdfDocument(List<Word> keywords)
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
            //return _lineList;
            return _lineList2;
        }

        public void ParseDocument(List<Word> keywords)
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
                CycleThrough(textList, lineCounter);//, keywords);
                SecondPass(keywords);
            }
            File.Delete(_tempDocumentPath);
        }

        public void SecondPass(List<Word> keywords)
        {
            //Cycle through list of contracts we built after seperating and combining sections
            foreach(Contract contract in _lineList)
            {
                //Cycle through Keywords
                foreach(Word word in keywords)
                {
                    if (!string.IsNullOrEmpty(contract.Data))
                    {
                        //If this section contains a keyword move on.
                        if (contract.Data.ToLower().Contains(word.Keyword.ToLower()))
                        {
                            //Change this to if split once we are done testing
                            if (word.Split)
                            {
                                //Split data into sentences
                                string[] sentences = Regex.Split(contract.Data, @"(?<=[\.!\?])\s+");
                                bool sentenceHit = false;
                                Contract newContract = new Contract();
                                newContract.DocumentSection = contract.DocumentSection;

                                foreach (var sentence in sentences)
                                {
                                    if (!string.IsNullOrEmpty(sentence.Trim()))
                                    {
                                        if(sentence.ToLower().Contains(word.Keyword.ToLower()))
                                        {
                                            //new section begins, close old one and start new
                                            sentenceHit = true;
                                            if(!string.IsNullOrEmpty(newContract.Data)) {
                                                _lineList2.Add(newContract);
                                                newContract = new Contract();
                                                newContract.Data = sentence;
                                                newContract.DocumentSection = contract.DocumentSection;
                                            }
                                        }
                                        else
                                        {
                                            //add to previous section
                                            sentenceHit = false;
                                            newContract.Data += sentence;
                                        }
                                    }
                                }                              
                            }
                            else 
                            {
                                //No splitting? Lets just add it to the new list.
                                _lineList2.Add(contract);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void CycleThrough(List<string> lines, int lineAmount)//, List<Word> keywords)
        {
            var lineCount = 0;
            var contract = new Contract();
            //bool keyWordHit = false;
            bool sameSection = false;

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
                if (!string.IsNullOrWhiteSpace(section.Trim()))
                {
                    if (section == _lastSectionId)
                    {
                        //We are in the same section
                        sameSection = true;
                    } 
                    else
                    {
                        if(contract.Data != "")// && keyWordHit)
                        {
                            _lineList.Add(contract);
                            contract = new Contract();
                            //keyWordHit = false;
                        }
                        _lastSectionId = section;
                        sameSection = false;
                    }
                } else
                {
                    if (!string.IsNullOrEmpty(_lastSectionId))
                    {
                        section = _lastSectionId;
                        sameSection = true;
                    } else
                    {
                        sameSection = false;
                        if (contract.Data != "")// && keyWordHit)
                        {
                            _lineList.Add(contract);
                            contract = new Contract();
                            //keyWordHit = false;
                        }
                    }
                }

                //Remove Section from current line
                if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(lineData) && lineData.Contains(section))
                {
                    lineData = lineData.Remove(0, section.Length + 1);
                }

                if(sameSection)
                {
                    contract.Data += lineData;
                } else {
                    //var newSentence = string.Empty;
                    contract.Data = lineData; //!string.IsNullOrEmpty(newSentence) ? newSentence : lineData;
                }
                
                contract.DocumentSection = _lastSectionId;
                //if (!string.IsNullOrEmpty(contract.Data.Trim()))
                //{
                //    foreach (Word keyword in keywords)
                //    {
                //        //Check for keyword
                //        if (contract.Data.ToLower().Contains(keyword.Keyword.ToLower()))
                //        {
                //            //Replace keywords with replacement words
                //            if (keyword.Replacement != "")
                //            {
                //                contract.Data = contract.Data.Replace(keyword.Keyword, keyword.Replacement);
                //            }
                //            keyWordHit = true;
                //            break;
                //        }
                //    }
                //}
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
