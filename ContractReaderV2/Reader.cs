using Code7248.word_reader;
using ContractReaderV2.Concrete;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if(!string.IsNullOrWhiteSpace(documentPath) && !string.IsNullOrWhiteSpace(tempPath))
            {
                _documentPath = documentPath;
                _tempDocumentPath = tempPath;
                _lineList = new List<Contract>();
            }
        }

        public List<Contract> ParseWordDocument()
        {
            var extractor = new TextExtractor(_documentPath);
            var docText = extractor.ExtractText();
            File.WriteAllText(_tempDocumentPath, docText);
            ParseTempDocument();
            return _lineList;

        }
        public List<Contract> ParsePdfDocument()
        {
            var wordList2 = new List<string>();
            if (File.Exists(_documentPath))
            {
                PdfReader pdfReader = new PdfReader(_documentPath);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    var words = new List<string>();
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
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
            ParseTempDocument();
            return _lineList;
        }

        public void ParseTempDocument()
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
                FirstPass(textList, 0, lineCounter);
            }
            File.Delete(_tempDocumentPath);
            
        }

        public void FirstPass(List<string> lines, int lineCount, int lineAmount, LineType lineType = LineType.Generic)
        {
            if (lineCount >= lineAmount) return;
            var contract = new Contract();
            var lineData = lines[lineCount];

            //Remove \t from doc
            if (lineData.StartsWith("\t"))
            {
                lineData = lineData.Replace("\t", "");
            }

            //Grab the section number if this line contains one
            var section = GetDocumentSection(lineData);

            //Set current section if we were able to find one.
            if(!string.IsNullOrWhiteSpace(section))
            {
                _lastSectionId = section;
            }
               
            //Remove Section from current line
            if (!string.IsNullOrEmpty(section))
            {
                lineData = lineData.Remove(0, section.Length);
            }

            //Look for keywords
            if (lines[lineCount].ToLower().Contains(GovShall))
            {
                contract.Data = lineData;
                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Government;
                _lineList.Add(contract);
                FirstPass(lines, lineCount + 1, lineAmount, LineType.Government);
            }
            else if (lines[lineCount].ToLower().Contains(GovWill))
            {
                contract.Data = lineData;
                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Government;
                _lineList.Add(contract);
                FirstPass(lines, lineCount + 1, lineAmount, LineType.Contractor);
            }
            else if (lines[lineCount].ToLower().Contains(ContractorShall))
            {
                lineData = Regex.Replace(lineData, ContractorShall,_parseHitReplace,RegexOptions.IgnoreCase);
                contract.Data = lineData;
                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Contractor;
                _lineList.Add(contract);
                FirstPass(lines, lineCount + 1, lineAmount, LineType.Contractor);
            }
            else if (lines[lineCount].ToLower().Contains(ContractorsShall))
            {
                lineData = Regex.Replace(lineData, ContractorsShall, _parseHitReplace, RegexOptions.IgnoreCase);
                contract.Data = lineData;
                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Contractor;
                _lineList.Add(contract);
                FirstPass(lines, lineCount + 1, lineAmount, LineType.Contractor);
            }
            else if (lines[lineCount].ToLower().Contains(ContractorWill))
            {
                lineData = Regex.Replace(lineData, ContractorWill, _parseHitReplace, RegexOptions.IgnoreCase);
                contract.Data = lineData;
                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Contractor;
                _lineList.Add(contract);
                FirstPass(lines, lineCount + 1, lineAmount, LineType.Contractor);
            }
            else
            {
                contract.Data = lineData;
                contract.DocumentSection = _lastSectionId;
                contract.DataType = LineType.Generic;
                FirstPass(lines, lineCount + 1, lineAmount);
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

    }
}
