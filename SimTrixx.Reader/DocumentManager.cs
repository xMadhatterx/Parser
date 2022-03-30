using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTrixx.Reader.Concrete;
using System.Text.RegularExpressions;
using System.IO;
using SimTrixx.Reader.Concrete.Enums;
using SimTrixx.Reader.Handlers;
namespace ContractReaderV2
{
    public class DocumentManager
    {
        private string _tempDocumentPath;
        private string _documentPath;

        public DocumentManager(string documentPath, string tempPath)
        {
            var documentType = new Handlers.FileExtensionHandler().GetDocumentType(documentPath);

            if (string.IsNullOrWhiteSpace(documentPath) || string.IsNullOrWhiteSpace(tempPath)) return;
            _tempDocumentPath = tempPath;
            _documentPath = documentPath;
            var documentHandler = new Handlers.TextExtractionHandler();
            if (documentType == GlobalEnum.DocumentType.WordDoc)
            {
                documentHandler.ParseWordDocument(_documentPath, _tempDocumentPath);
                //documentHandler.ParseWordInterop(_documentPath, _tempDocumentPath);
            }
            if (documentType == GlobalEnum.DocumentType.Pdf)
            {
                documentHandler.ParsePdfDocument(_documentPath, _tempDocumentPath);
            }

            if(documentType == GlobalEnum.DocumentType.NotSupported)
            {
                throw new Exception("unsupported document type");
            }
        }

        public List<Contract> ParseDocument(BindingList<Word> keywords,GlobalEnum.DocumentParseMode documentParseMode,bool advancedFiltering)
        {
            var textList = new List<string>();
            var lineCounter = 0;
            var abbrvHandler = new AbbrvExtractionHandler();
            var abbrvList = new List<AbbrvContainer>();
            var contractList = new List<Contract>();
            
            using (var reader = new StreamReader(new FileStream(_tempDocumentPath, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    textList.Add(reader.ReadLine());
                    lineCounter++;
                }
            }

            textList = SentenceHandler.GetSentences(textList.ToList());

            foreach (var actionResult in textList.Select(sentence => abbrvHandler.GetAbbreviations(sentence)).Where(actionResult => actionResult.Count > 0))
            {
                abbrvList.AddRange(abbrvHandler.CreateAbbrvEntity(actionResult));
            }

            switch (documentParseMode)
            {
                case GlobalEnum.DocumentParseMode.FullDocument:
                    contractList = CreateFullDocument(textList, lineCounter, advancedFiltering);
                    break;
                case GlobalEnum.DocumentParseMode.KeyWordSectionsOnly:
                    contractList = CreateDocumentWithKeywordSections(textList, lineCounter, keywords, advancedFiltering);
                    break;
                case GlobalEnum.DocumentParseMode.KeyWordSectionsWithSplits:
                    contractList = CreateDocumentWithKeywordSectionsSplits(textList, lineCounter, keywords, advancedFiltering);
                    break;
                default:
                    throw new Exception("Unsupported document parsing mode");
            }
            return contractList;
        }

        public List<AbbrvContainer> GetAbbriviations()
        {
            var textList = new List<string>();
            var lineCounter = 0;
            var abbrvHandler = new AbbrvExtractionHandler();
            var mainAbbrList = new List<AbbrvContainer>();
            var abbrvList = new List<AbbrvContainer>();
            var contractList = new List<Contract>();
            
            using (var reader = new StreamReader(new FileStream(_tempDocumentPath, FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    textList.Add(reader.ReadLine());
                    lineCounter++;
                }
            }

            textList = SentenceHandler.GetSentences(textList.ToList());

            foreach (var actionResult in textList.Select(sentence => abbrvHandler.GetAbbreviations(sentence)).Where(actionResult => actionResult.Count > 0))
            {
                abbrvList.AddRange(abbrvHandler.CreateAbbrvEntity(actionResult));
            }

            foreach (var abbrv in abbrvList)
            {
                if (mainAbbrList.Exists(x => x.Abbrv == abbrv.Abbrv))
                {
                    var k = mainAbbrList.FirstOrDefault(x => x.Abbrv == abbrv.Abbrv);
                    k.Count = k.Count + 1;
                    k.Location.AddRange(abbrv.Location);
                }
                else
                {
                    mainAbbrList.Add(abbrv);

                }
            }

            mainAbbrList= mainAbbrList.OrderBy(x => x.Abbrv).ToList();
            return mainAbbrList;
        }

        private List<Contract> CreateFullDocument(List<string> lines, int totalLines,bool advancedFiltering)
        {
            var sectionHandler = new Handlers.SectionHandler();
            File.Delete(_tempDocumentPath);
            return sectionHandler.GetSections(lines, totalLines,advancedFiltering);
        }

        private List<Contract> CreateDocumentWithKeywordSections(List<string> lines, int totalLines,BindingList<Word> keywords, bool advancedFiltering)
        {
            var sectionHandler = new Handlers.SectionHandler();
            var sectionFilterHandler = new Handlers.SectionFilterHandler();
            var fullContractList = sectionHandler.GetSections(lines, totalLines,advancedFiltering);
            var contractListKeywordSectionOnly = sectionFilterHandler.GetSectionsWithKeywords(keywords,fullContractList);
            File.Delete(_tempDocumentPath);
            return contractListKeywordSectionOnly;
        }

        private List<Contract> CreateDocumentWithKeywordSectionsSplits(List<string> lines, int totalLines, BindingList<Word> keywords, bool advancedFiltering)
        {
            var sectionHandler = new Handlers.SectionHandler();
            var sectionFilterHandler = new Handlers.SectionFilterHandler();
            var fullContractList = sectionHandler.GetSections(lines, totalLines,advancedFiltering);
            var contractListKeywordSectionOnly = sectionFilterHandler.GetSectionsWithKeywords(keywords,fullContractList);
            var contractWithSectionSplits = sectionFilterHandler.SplitSectionsByKeyword(contractListKeywordSectionOnly, keywords);
            File.Delete(_tempDocumentPath);
            return contractWithSectionSplits;
        }




       
    }
}
