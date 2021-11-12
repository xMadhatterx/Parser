using System.Collections.Generic;
using SimTrixx.Reader.Concrete;
using Code7248.word_reader;
using System.IO;
using System.Text.RegularExpressions;
using static SimTrixx.Reader.Concrete.Enum.GlobalEnum;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using com.sun.corba.se.spi.orbutil.threadpool;
using System.Linq;
using com.sun.corba.se.spi.orbutil.fsm;
using System;
using static System.Version;

namespace ContractReaderV2
{
    public class ReaderV2
    {
        private readonly string _documentPath;
        private readonly string _tempDocumentPath;
        private readonly List<Contract> _lineList;
        private readonly List<Contract> _lineList2;
        private string _lastSectionId;
        private bool _inActiveSection = false;
        private bool _atleastOne = false;

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
                GetSections(textList, lineCounter);
                var keywordSections = GetSectionsWithKeywords(keywords);
                _lineList2.AddRange(SplitSectionsByKeyword(keywordSections, keywords));

                //CycleThrough(textList, lineCounter, keywords);
                //GetKeywords(keywords);
            }
            File.Delete(_tempDocumentPath);
        }

        public void GetSections(List<string> lines, int lineAmount)
        {
            var lineCount = 0;
            var contract = new Contract();
            bool sameSection = false;

            while (true)
            {
                //If we are on the last line then wrap it up.
                if (lineCount == lineAmount)
                {
                    if (!string.IsNullOrEmpty(contract.Data))
                    {
                        _lineList.Add(contract);
                    }
                    return;
                }

                //If we are 1 past the last line then wrap it up
                if (lineCount > lineAmount) return;

                var lineData = lines[lineCount];
                if (!string.IsNullOrEmpty(lineData))
                {
                    //Remove \f from doc
                    if (lineData.StartsWith("\f"))
                    {
                        lineData = lineData.Replace("\f", "");
                    }

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

                    //Get section is we have one on this line
                    var section = GetNewDocumentSection(lineData);

                    //Work out section magic
                    sameSection = false;
                    if (!string.IsNullOrWhiteSpace(section.Trim()) && section != "consolidate")
                    {
                        //Section Found
                        //-------------
                        //Check if we have a previous section id
                        if (!string.IsNullOrEmpty(_lastSectionId))
                        {
                            //If we do compare the current section to the last section
                            if (section == _lastSectionId)
                            {
                                //boom - same section
                                sameSection = true;
                            }
                            else
                            {
                                try
                                {
                                    var workingSection = section;
                                    var workingLastSection = _lastSectionId;
                                    if (workingSection.Length == 1) workingSection = workingSection + ".0";
                                    if (workingLastSection.Length == 1) workingLastSection = workingLastSection + ".0";
                                    var newSection = new Version(workingSection);
                                    var lastSection = new Version(workingLastSection);

                                    var result = newSection.CompareTo(lastSection);
                                    if (result > 0)
                                        _lastSectionId = section.Trim();
                                    else if (result < 0)
                                        sameSection = true;
                                    else
                                        sameSection = true;
                                } catch {
                                    var newSection = section.Replace(".", "");
                                    var newLastSection = _lastSectionId.Replace(".", "");
                                    Int32.TryParse(newSection, out int newSectionDecimal);
                                    Int32.TryParse(newLastSection, out int lastSectionDecimal);

                                    if (newSectionDecimal < lastSectionDecimal)
                                    {
                                        sameSection = true;
                                    }
                                    else
                                    {
                                        //New Section, let's set our sectionid
                                        _lastSectionId = section.Trim();
                                    }
                                }
                                
                            }
                        }
                        else
                        {
                            //New Section, let's set our _lastSectionId
                            _lastSectionId = section.Trim();
                        }
                    }
                    else
                    {
                        //Section Not Found
                        //-----------------
                        //Check if we are consolidating
                        if (section == "consolidate")
                        {
                            _atleastOne = true;
                            _lineList.Add(contract);
                            contract = new Contract();
                        }
                        else
                        {
                            //Check if we have a previous section id
                            if (!string.IsNullOrEmpty(_lastSectionId) && _inActiveSection)
                            {
                                //We have a previous section, let's use that
                                sameSection = true;
                            } else
                            {
                                _lastSectionId = section.Trim();
                            }
                        }
                    }

                    //Remove Section from current line
                    if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(lineData) && lineData.Contains(section))
                    {
                        lineData = lineData.Remove(0, section.Length + 1);
                    }

                    //Lets check and see if we are in a new section or a continuation section
                    if (sameSection)
                    {
                        //Same section as previous
                        //------------------------
                        if (!string.IsNullOrEmpty(lineData))
                            contract.Data += lineData;

                    }
                    else
                    {
                        //New section
                        //------------
                        //If we have data sitting in contract we need to add it to the linedata and create a new instance.
                        if (!string.IsNullOrEmpty(contract.Data))
                        {
                            _lineList.Add(contract);
                            contract = new Contract();
                        }
                        //Add line data to contract
                        if (!string.IsNullOrEmpty(_lastSectionId) && section != "consolidate" && _inActiveSection)
                        {
                            contract.DocumentSection = _lastSectionId;
                            contract.Data = lineData;
                        }
                    }
                }
                lineCount++;
            }
        }
        
        public List<Contract> GetSectionsWithKeywords(List<Word> keywords)
        {
            var keywordSection = new List<Contract>();
            foreach(var contract in _lineList)
            {
                foreach (var keyword in keywords)
                {
                    if(contract.Data.ToLower().Contains(keyword.Keyword.ToLower()))
                    {
                        keywordSection.Add(contract);
                        break;
                    }
                }
            }
            return keywordSection;
        }

        public List<Contract> SplitSectionsByKeyword(List<Contract> contracts,List<Word> keywords)
        {
            var splitSectionContract = new List<Contract>();
            foreach(var contract in contracts)
            {
                string[] sentences = Regex.Split(contract.Data, @"(?<=[\.!\?])\s+");
                var indexes = new List<int>();
                //indexes.Add(0);
                if(contract.DocumentSection=="4.1")
                {
                    var j = 1;
                }
                foreach (var sentence in sentences)
                {

                    foreach(var word in keywords)
                    {
                        if (sentence.ToLower().Contains(word.Keyword.ToLower()))
                        {
                            var index = contract.Data.IndexOf(sentence);
                            //Just to test the return of the index
                            var t = contract.Data.Substring(contract.Data.IndexOf(sentence), 1);
                            if (!indexes.Contains(index))
                            {
                                indexes.Add(index);
                                break;

                            }

                        }
                    }
                    if (indexes.Count > 0 && !indexes.Contains(0))
                    {
                        indexes.Add(0);
                    }

                }

                var orderedIndexes = indexes.OrderBy(x => x).ToList();
                if (orderedIndexes.Count >= 1)
                {
                    for (var i = 0; i < orderedIndexes.Count; i++)
                    {
                        var k = new Contract();
                        k.DocumentSection = contract.DocumentSection;
                        if (i != orderedIndexes.Count -1)
                        {
                            var m = orderedIndexes[i];
                            var l = orderedIndexes[(i + 1)] - 1;
                            var j = (orderedIndexes[i + 1] - orderedIndexes[i])-1;
                            k.Data = contract.Data.Substring(orderedIndexes[i], j);
                            splitSectionContract.Add(k);
                        }
                        else
                        {
                            var j = ((contract.Data.Length - 1) - orderedIndexes[i]);
                            k.Data = contract.Data.Substring(orderedIndexes[i], j);
                            splitSectionContract.Add(k);
                        }

                    }
                }
                Contract newContract = new Contract();
                newContract.DocumentSection = contract.DocumentSection;
            }
            return splitSectionContract;
        }

        //public void GetKeywords(List<Word> keywords)
        //{
        //    //Cycle through list of contracts we built after seperating and combining sections
        //    foreach (Contract contract in _lineList)
        //    {
        //        if (!string.IsNullOrEmpty(contract.Data))
        //        {
        //            string[] sentences = Regex.Split(contract.Data, @"(?<=[\.!\?])\s+");
        //            Contract newContract = new Contract();
        //            newContract.DocumentSection = contract.DocumentSection;

        //            var sbSentence = new StringBuilder();

        //            int sentenceCount = sentences.Length;
        //            int currentCount = 0;
        //            bool firstKeyword = false;

        //            List<string> keys = new List<string>();
        //            //Put keywords into string array
        //            foreach (Word word in keywords)
        //            {
        //                keys.Add(word.ToString());
        //            }

        //            foreach (var sentence in sentences)
        //            {
        //                currentCount++;
        //                if (!string.IsNullOrEmpty(sentence.Trim()))
        //                {
        //                    //Check to see if this sentence has a keyword in it
        //                    bool keywordHit = keywords.Any(k => sentence.ToLower().Contains(k.Keyword.ToLower()));
        //                    if (keywordHit)
        //                    {
        //                        //Remove previous sentences if this is the first keyword hit
        //                        if (!firstKeyword)
        //                        {
        //                            sbSentence = new StringBuilder();
        //                            firstKeyword = true;
        //                        }
        //                        //new section begins, close old one and start new
        //                        if (sbSentence.Length != 0)
        //                        {
        //                            newContract.Data = sbSentence.ToString();
        //                            foreach (var keyword in keywords)
        //                            {
        //                                newContract.Data = Regex.Replace(newContract.Data, keyword.Keyword, keyword.Replacement, RegexOptions.IgnoreCase);
        //                            }
        //                            _lineList2.Add(newContract);
        //                            newContract = new Contract();
        //                            newContract.DocumentSection = contract.DocumentSection;
        //                        }
        //                        sbSentence = new StringBuilder();
        //                        sbSentence.Append(sentence);
        //                    }
        //                    else
        //                    {
        //                        //add to previous section
        //                        if (firstKeyword)
        //                        {
        //                            sbSentence.Append(sentence);
        //                        }
        //                    }
        //                }
        //                //Add what we have left if we hit here and still have data in out stringbuilder
        //                if (currentCount == sentenceCount && sbSentence.Length != 0)
        //                {
        //                    newContract.Data = sbSentence.ToString();
        //                    foreach (var keyword in keywords)
        //                    {
        //                        newContract.Data = Regex.Replace(newContract.Data, keyword.Keyword, keyword.Replacement, RegexOptions.IgnoreCase);
        //                    }
        //                    _lineList2.Add(newContract);
        //                    newContract = new Contract();
        //                    newContract.DocumentSection = contract.DocumentSection;
        //                }
        //            }
        //        }
        //    }
        //}

        //public void CycleThrough(List<string> lines, int lineAmount, List<Word> keywords)
        //{
        //    var lineCount = 0;
        //    var contract = new Contract();
        //    bool keyWordHit = false;
        //    bool sameSection = false;

        //    while (true)
        //    {
        //        if (lineCount >= lineAmount) return;
        //        var lineData = lines[lineCount];
        //        if (!string.IsNullOrEmpty(lineData))
        //        {

        //            //Remove \t from doc
        //            if (lineData.StartsWith("\t"))
        //            {
        //                lineData = lineData.Replace("\t", "");
        //            }

        //            //Second check to replace any mid line tabs with spaces.
        //            if (lineData.Contains("\t"))
        //            {
        //                lineData = lineData.Replace("\t", " ");
        //            }

        //            //Grab the section number if this line contains one
        //            var section = GetDocumentSection(lineData);

        //            //Set current section if we were able to find one.
        //            if (!string.IsNullOrWhiteSpace(section.Trim()))
        //            {
        //                if (section == _lastSectionId)
        //                {
        //                    //We are in the same section
        //                    sameSection = true;
        //                }
        //                else
        //                {
        //                    if (contract.Data != "" && keyWordHit)
        //                    {
        //                        _lineList.Add(contract);
        //                        contract = new Contract();
        //                        keyWordHit = false;
        //                    }
        //                    _lastSectionId = section;
        //                    sameSection = false;
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(_lastSectionId))
        //                {
        //                    section = _lastSectionId;
        //                    sameSection = true;
        //                }
        //                else
        //                {
        //                    sameSection = false;
        //                    if (contract.Data != "" && keyWordHit)
        //                    {
        //                        _lineList.Add(contract);
        //                        contract = new Contract();
        //                        keyWordHit = false;
        //                    }
        //                }
        //            }

        //            //Remove Section from current line
        //            if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(lineData) && lineData.Contains(section))
        //            {
        //                lineData = lineData.Remove(0, section.Length + 1);
        //            }

        //            if (sameSection)
        //            {
        //                contract.Data += lineData;
        //            }
        //            else
        //            {
        //                //var newSentence = string.Empty;
        //                contract.Data = lineData; //!string.IsNullOrEmpty(newSentence) ? newSentence : lineData;
        //            }

        //            contract.DocumentSection = _lastSectionId;
        //            //if (!string.IsNullOrEmpty(contract.Data.Trim()))
        //            //{
        //            //    foreach (Word keyword in keywords)
        //            //    {
        //            //        //Check for keyword
        //            //        if (contract.Data.ToLower().Contains(keyword.Keyword.ToLower()))
        //            //        {
        //            //            //Replace keywords with replacement words
        //            //            //if (keyword.Replacement != "")
        //            //            //{
        //            //            //    contract.Data = Regex.Replace(contract.Data, keyword.Keyword, keyword.Replacement, RegexOptions.IgnoreCase);
        //            //            //contract.Data = result;// contract.Data.Replace(keyword.Keyword, keyword.Replacement);
        //            //            //}
        //            //            keyWordHit = true;
        //            //            break;
        //            //        }
        //            //    }
        //            //}
        //        }
        //        lineCount++;
        //    }
        //}

        private string GetNewDocumentSection(string line)
        {
            var section = string.Empty;
            bool dotPresent = false;

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
            if (!match && !_inActiveSection) return section;

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
                        if (_inActiveSection)
                        {
                            if (!string.IsNullOrEmpty(section) && section.ToString() != " ")
                            {
                                //We hit the next section after a section was filled in
                                //_inActiveSection = false;
                                return section;
                                //return "consolidate";
                            } 
                            else
                            {
                                _inActiveSection = false;
                                return "consolidate";
                            }
                        }
                        _inActiveSection = true;
                        if (dotPresent)
                        {
                            return section;
                        }
                        else
                        {
                            return section;
                        }
                    }
                    if (t.ToString() == ".")
                        dotPresent = true;
                    var isNumber = int.TryParse(t.ToString(), out _);
                    if (isNumber || t.ToString() == ".")
                    {
                        section += t.ToString();
                    }
                }
            }

            if (dotPresent)
            {
                return section;
            }
            else
            {
                return "";
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

            //Return blank if no section tag
            //if (!line.ToLower().Contains("~section~")) return section;
            
            //Return blank if no match
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
