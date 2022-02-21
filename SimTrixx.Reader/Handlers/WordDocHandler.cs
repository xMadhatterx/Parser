using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Spire.Doc;

namespace ContractReaderV2.Handlers
{
    public class WordDocHandler
    {
        public string TextFromWord(string filePath)
        {
            StringBuilder textBuilder = new StringBuilder();

            try
            {
                Stream stream = File.OpenRead(filePath);
                Document doc = new Document(stream);
                stream.Close();
                var path = Path.GetTempFileName();
                doc.SaveToTxt(path, Encoding.UTF8);
                textBuilder.Append(File.ReadAllText(path));
                
                
            } catch (Exception ex)
            {
                
            }
            return textBuilder.ToString();
        }
    }
}
