using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTrixx.Reader.Concrete
{
    public class Root
    {
        public BindingList<Word> Keywords { get; set; }
    }
    public class Word
    {
        public string Keyword { get; set; }
        public string Replacement { get; set; }
        public bool Split { get; set; }
    }
}
