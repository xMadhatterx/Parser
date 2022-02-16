using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using Code7248.word_reader;
using System.IO;
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
