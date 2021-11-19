namespace SimTrixx.Reader.Concrete.Enums
{
   public class GlobalEnum
    {
        public enum LineType
        {
            Government,
            Contractor,
            Generic,
        }

        public enum DocumentType
        {
            WordDoc,
            Pdf,
            NotSupported
        }
        public enum DocumentParseMode
        {
            FullDocument,
            KeyWordSectionsOnly,
            KeyWordSectionsWithSplits
        }
    }
}
