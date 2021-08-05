using static SimTrixx.Reader.Concrete.Enum.GlobalEnum;

namespace SimTrixx.Reader.Concrete
{
   public  class Contract
    {
        public string DocumentSection { get; set; }
        public LineType DataType { get; set; }
        public string Data { get; set; }
        
    }
}
