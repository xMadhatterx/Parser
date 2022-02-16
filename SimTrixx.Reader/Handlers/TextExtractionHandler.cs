using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using Code7248.word_reader;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Xml;
using System;
using System.Xml.Linq;
using System.Linq;
//using Spire.Doc;
//using Spire.Doc.Documents;
namespace ContractReaderV2.Handlers
{
   public class TextExtractionHandler
    {
        public void ParseWordDocument(string documentPath, string tempDocumentPath)
        {
            var docText = new WordDocHandler().TextFromWord(documentPath);
            //var extractor = new TextExtractor(documentPath);
            //var docText = extractor.ExtractText();
            File.WriteAllText(tempDocumentPath, docText);
        }

        public void ParseWordInterop(string documentPath, string tempDocumentPath)
        {
            //var document = new Document();
            //document.LoadFromFile(documentPath);
            //var outputPath = $@"{Path.GetDirectoryName(documentPath)}\sample.xml";
            //document.SaveToFile(outputPath, FileFormat.Xml);

            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object oMissing = System.Reflection.Missing.Value;

            word.Visible = false;
            word.ScreenUpdating = false;

            XmlDocument xmlDoc = new XmlDocument();

                Document doc = word.Documents.Open(documentPath, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                doc.Activate();

                
                object fileFormat = WdSaveFormat.wdFormatXML;
            var outputPath = $@"{Path.GetDirectoryName(documentPath)}\sample.xml";
            doc.SaveAs(outputPath, ref fileFormat, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                     ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
                doc = null;

            
             xmlDoc.Load(outputPath.ToString());
            var xmlString = RemoveAllNamespaces(xmlDoc.OuterXml);
            //    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            //nsmgr.RemoveNamespace("w", "http://schemas.microsoft.com/office/word/2003/wordml");
                
               // nsmgr.AddNamespace("w", "http://schemas.microsoft.com/office/word/2003/wordml");

               // XmlNodeList node = xmlDoc.SelectNodes("//w:wordDocument/descendant::w:t|//w:wordDocument/descendant::w:p|//w:wordDocument/descendant::w:tab", nsmgr);

            ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
                word = null;

        }

        public string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        public XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

        public void ParsePdfDocument(string documentPath, string tempDocumentPath)
        {
            if (File.Exists(documentPath))
            {
                var doc = PDDocument.load(documentPath);
                var textStrip = new PDFTextStripper();
                var strPdfText = textStrip.getText(doc);
                doc.close();
                File.WriteAllText(tempDocumentPath, strPdfText);
            }
        }
    }
}
