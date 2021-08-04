using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractReaderV2.Concrete
{
    public class Root
    {
        public List<Word> Keywords { get; set; }
    }
    public class Word
    {
        public string Keyword { get; set; }
        public string Replacement { get; set; }
        public bool Split { get; set; }
    }
}
