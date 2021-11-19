using SimTrixx.Reader.Concrete.Enums;
namespace ContractReaderV2.Handlers
{
    public class FileExtensionHandler
    {
       

        public GlobalEnum.DocumentType GetDocumentType(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName);
            if(extension == ".doc")
            {
                return GlobalEnum.DocumentType.WordDoc;
            }
            else if(extension == ".docx")
            {
                return GlobalEnum.DocumentType.WordDoc;
            }
            else if(extension == ".pdf")
            {
                return GlobalEnum.DocumentType.Pdf;
            }
            else
            {
                return GlobalEnum.DocumentType.NotSupported;
            }
        }

    }
}
