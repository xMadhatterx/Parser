using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTrixx.Reader.Concrete;
using System.Text.RegularExpressions;
using System.IO;
namespace ContractReaderV2
{
    public class ReaderV3
    {
        public enum DocumentType
        {
            Word,
            Pdf
        }

        public enum DocumentParseMode
        {
            FullDocument,
            KeyWordSectionsOnly,
            KeyWordSectionsWithSplits
        }

        private string _tempDocumentPath;
        private string _documentPath;

        public ReaderV3(string documentPath, string tempPath,DocumentType documentType)
        {
            if (string.IsNullOrWhiteSpace(documentPath) || string.IsNullOrWhiteSpace(tempPath)) return;
            _tempDocumentPath = tempPath;
            _documentPath = documentPath;
            var documentHandler = new Handlers.DocumentHandler();
            if (documentType == DocumentType.Word)
            {
                documentHandler.ParseWordDocument(_documentPath, _tempDocumentPath);
            }
            else if (documentType == DocumentType.Pdf)
            {
                documentHandler.ParsePdfDocument(_documentPath, _tempDocumentPath);
            }
            else
            {
                throw new Exception("unsupported document type");
            }
        }
        public List<Contract> ParseDocument(List<Word> keywords,DocumentParseMode documentParseMode)
        {
            
            var textList = new List<string>();
            var lineCounter = 0;
            var contractList = new List<Contract>();
            using (var reader = new StreamReader(new FileStream(_tempDocumentPath, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    textList.Add(reader.ReadLine());
                    lineCounter++;
                }
            }
                
                if(documentParseMode == DocumentParseMode.FullDocument)
                {
                    contractList= CreateFullDocument(textList, lineCounter);
                }
                else if(documentParseMode == DocumentParseMode.KeyWordSectionsOnly)
                {
                    contractList= CreateDocumentWithKeywordSections(textList, lineCounter, keywords);
                }
                else if(documentParseMode == DocumentParseMode.KeyWordSectionsWithSplits)
                {
                    contractList = CreateDocumentWithKeywordSectionsSplits(textList, lineCounter, keywords);
                }
                else
                {
                    throw new Exception("Unsupported document parsing mode");
                }
                return contractList;
            
            
        }

        private List<Contract> CreateFullDocument(List<string> lines, int totalLines)
        {
            var sectionHandler = new Handlers.SectionHandler();
            File.Delete(_tempDocumentPath);
            return sectionHandler.GetSections(lines, totalLines);
        }

        private List<Contract> CreateDocumentWithKeywordSections(List<string> lines, int totalLines,List<Word> keywords)
        {
            var sectionHandler = new Handlers.SectionHandler();
            var fullContractList = sectionHandler.GetSections(lines, totalLines);
            var contractListKeywordSectionOnly = GetSectionsWithKeywords(keywords,fullContractList);
            File.Delete(_tempDocumentPath);
            return contractListKeywordSectionOnly;
        }

        private List<Contract> CreateDocumentWithKeywordSectionsSplits(List<string> lines, int totalLines, List<Word> keywords)
        {
            var sectionHandler = new Handlers.SectionHandler();
            var fullContractList = sectionHandler.GetSections(lines, totalLines);
            var contractListKeywordSectionOnly = GetSectionsWithKeywords(keywords,fullContractList);
            var contractWithSectionSplits = SplitSectionsByKeyword(contractListKeywordSectionOnly, keywords);
            File.Delete(_tempDocumentPath);
            return contractWithSectionSplits;
        }



        private List<Contract> GetSectionsWithKeywords(List<Word> keywords,List<Contract> contractLines)
        {
            var keywordSection = new List<Contract>();
            foreach (var contract in contractLines)
            {
                foreach (var keyword in keywords)
                {
                    if (contract.Data.ToLower().Contains(keyword.Keyword.ToLower()))
                    {
                        keywordSection.Add(contract);
                        break;
                    }
                }
            }
            return keywordSection;
        }
        private List<Contract> SplitSectionsByKeyword(List<Contract> contracts, List<Word> keywords)
        {
            var splitSectionContract = new List<Contract>();
            foreach (var contract in contracts)
            {
                string[] sentences = Regex.Split(contract.Data, @"(?<=[\.!\?])\s+");
                var indexes = new List<int>();
                //indexes.Add(0);
                if (contract.DocumentSection == "4.1")
                {
                    var j = 1;
                }
                foreach (var sentence in sentences)
                {

                    foreach (var word in keywords)
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
                        if (i != orderedIndexes.Count - 1)
                        {
                            var m = orderedIndexes[i];
                            var l = orderedIndexes[(i + 1)] - 1;
                            var j = (orderedIndexes[i + 1] - orderedIndexes[i]) - 1;
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
    }
}
