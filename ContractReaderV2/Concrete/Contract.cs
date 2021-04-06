using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ContractReaderV2.Concrete.Enum.GlobalEnum;

namespace ContractReaderV2.Concrete
{
   public  class Contract
    {
        public string DocumentSection { get; set; }
        public LineType DataType { get; set; }
        public string Data { get; set; }
        
    }
}
