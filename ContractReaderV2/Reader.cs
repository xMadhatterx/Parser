using Code7248.word_reader;
using ContractReaderV2.Concrete;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static ContractReaderV2.Concrete.Enum.GlobalEnum;

namespace ContractReaderV2
{
    public class Reader
    {
        private string _documentPath;
        private string _tempDocumentPath;
        private List<Contract> _lineList;
        private const string CONTRACTOR_WILL = "contractor will";
        private const string CONTRACTOR_SHALL = "contractor shall";
        private const string CONTRACTORS_SHALL = "contractors shall";
        private const string GOV_WILL = "the government will";
        private const string GOV_SHALL = "the government shall";
        private string _lastSectionId;
        private string _parseHitReplace = "My company name";




        public Reader(string documentPath,string tempPath,DocumentType documentType)
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
            //var file = new FileInfo(fullTempPath).Create();
            File.WriteAllText(_tempDocumentPath, docText);
            ParseTempDocument();
            return _lineList;

        }
        public List<Contract> ParsePdfDocument()
        {
            StringBuilder text = new StringBuilder();
            
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
                    for (int j = 0, len = words.Count(); j < len; j++)
                    {
                        wordList2.Add( Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(words[j]))));
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
                WriteText(textList, 0, lineCounter);
            }
            File.Delete(_tempDocumentPath);
            
        }

        public void WriteText(List<string> lines, int lineCount, int lineAmount, LineType lineType = LineType.Generic)
        {

            if (lineCount < lineAmount)
            {


                //var line = lines[lineCount].ToLower();

                // if (line.Contains("contractors shall"))
                //{
                //    richTextBox1.Text += $"{lines[lineCount]}{System.Environment.NewLine}";
                //    WriteText(lines, lineCount +1, lineAmount, LineType.Contractor);
                //}
                //else
                //{
                //    richTextBox3.Text += $"{lines[lineCount]}{System.Environment.NewLine}";
                //    WriteText(lines, lineCount+ 1, lineAmount, LineType.Generic);
                //}

                //if (!lines[lineCount].ToLower().Contains("government shall") || lines[lineCount].ToLower().Contains("contractor shall"))
                //{
                //    if (lineType == LineType.Government)
                //    {
                //        richTextBox1.Text += $"{lines[lineCount]}{System.Environment.NewLine}";
                //        WriteText(lines, lineCount +1, lineAmount, LineType.Government);
                //    }
                //    else if (lineType == LineType.Contractor)
                //    {

                //        richTextBox2.Text += $"{lines[lineCount]}{System.Environment.NewLine}";
                //        WriteText(lines, lineCount +1, lineAmount, LineType.Contractor);
                //    }
                //    else
                //    {
                //        richTextBox3.Text += $"{lines[lineCount]}{System.Environment.NewLine}";
                //        WriteText(lines, lineCount +1, lineAmount, LineType.Generic);
                //    }
                //}

                var contract = new Contract();
                var lineData = lines[lineCount];
                if (lineData.StartsWith("\t"))
                {
                    lineData = lineData.Replace("\t", "");
                }
                var section = GetDocumentSection(lineData);
                if(!string.IsNullOrWhiteSpace(section))
                {
                    _lastSectionId = section;
                }
               
                    if (section != string.Empty)
                    {
                        lineData = lineData.Remove(0, section.Count());
                    }

                if (lines[lineCount].ToLower().Contains(GOV_SHALL))
                {
                    contract.Data = lineData;
                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Government;
                    _lineList.Add(contract);
                    WriteText(lines, lineCount + 1, lineAmount, LineType.Government);
                }
                else if (lines[lineCount].ToLower().Contains(GOV_WILL))
                {
                    contract.Data = lineData;
                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Government;
                    _lineList.Add(contract);
                    WriteText(lines, lineCount + 1, lineAmount, LineType.Contractor);
                }
                else if (lines[lineCount].ToLower().Contains(CONTRACTOR_SHALL))
                {
                    lineData = Regex.Replace(lineData, CONTRACTOR_SHALL,_parseHitReplace,RegexOptions.IgnoreCase);
                    contract.Data = lineData;
                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Contractor;
                    _lineList.Add(contract);
                    WriteText(lines, lineCount + 1, lineAmount, LineType.Contractor);
                }
                else if (lines[lineCount].ToLower().Contains(CONTRACTORS_SHALL))
                {


                    lineData = Regex.Replace(lineData, CONTRACTORS_SHALL, _parseHitReplace, RegexOptions.IgnoreCase);
                    contract.Data = lineData;
                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Contractor;
                    _lineList.Add(contract);
                    WriteText(lines, lineCount + 1, lineAmount, LineType.Contractor);
                }
                else if (lines[lineCount].ToLower().Contains(CONTRACTOR_WILL))
                {
                    lineData = Regex.Replace(lineData, CONTRACTOR_WILL, _parseHitReplace, RegexOptions.IgnoreCase);
                    contract.Data = lineData;
                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Contractor;
                    _lineList.Add(contract);
                    WriteText(lines, lineCount + 1, lineAmount, LineType.Contractor);
                }
                else
                {

                    contract.Data = lineData;
                    contract.DocumentSection = _lastSectionId;
                    contract.DataType = LineType.Generic;
                    //_lineList.Add(contract);
                    WriteText(lines, lineCount + 1, lineAmount, LineType.Generic);
                }


            }

        }

        public string GetDocumentSection(string line)
        {
            var charArray = line.ToArray();
            string section = string.Empty;
         
            for(var k = 0; k < line.Length;k++)
            {
                int sectionNumber;
               
                var isNumber = int.TryParse(line[k].ToString(), out sectionNumber);
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
