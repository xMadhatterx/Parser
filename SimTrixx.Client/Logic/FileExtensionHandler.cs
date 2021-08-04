namespace TestDocReader.Logic
{
    public class FileExtensionHandler
    {
        //public FileExtensionHandler()
        //{
        //    var fileExtensions = new List<string> {".doc", ".docx", ".pdf"};
        //}

        public enum FileType
        {
            WordDoc,
            Pdf,
            NotSupported
        }

        public FileType GetDocumentType(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName);
            if(extension == ".doc")
            {
                return FileType.WordDoc;
            }
            else if(extension == ".docx")
            {
                return FileType.WordDoc;
            }
            else if(extension == ".pdf")
            {
                return FileType.Pdf;
            }
            else
            {
                return FileType.NotSupported;
            }
        }

    }
}
