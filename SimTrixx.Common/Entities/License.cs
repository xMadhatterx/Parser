using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTrixx.Common.Entities
{
    public class License
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Expiration { get; set; }
        public int Quantity { get; set; }
        public Customer Customer { get; set; }
        public string Signature { get; set; }
    }
}
