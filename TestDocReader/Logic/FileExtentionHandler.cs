using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDocReader.Logic
{
    public class FileExtentionHandler
    {
        private List<string> _fileExtentions;
        public FileExtentionHandler()
        {
            _fileExtentions = new List<string>();
            _fileExtentions.Add(".doc");
            _fileExtentions.Add(".docx");
            _fileExtentions.Add(".pdf");
        }

        public enum FileType
        {
            WordDoc,
            Pdf,
            NotSupported
        }

        public FileType GetDocumentType(string fileName)
        {
            var extention = System.IO.Path.GetExtension(fileName);
            if(extention == ".doc")
            {
                return FileType.WordDoc;
            }
            else if(extention == ".docx")
            {
                return FileType.WordDoc;
            }
            else if(extention == ".pdf")
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
