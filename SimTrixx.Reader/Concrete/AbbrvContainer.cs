using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTrixx.Reader.Concrete
{
   public class AbbrvContainer
    {
        public string Abbrv { get; set; }
        public string Definition { get; set; }
        public int Count { get; set; }
        public List<string> Location { get; set; }
    }
}
