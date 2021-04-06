using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractReaderV2.Concrete.Enum
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
            doc,
            pdf
        }
    }
}
