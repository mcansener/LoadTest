using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTest
{
    public class RequestConfig
    {
        public string RequestUrl { get; set; }
        public string RequestMethod { get; set; }
        public Dictionary<string, string> RequestHeaders { get; set; }
        public Dictionary<string, object> RequestBody { get; set; }
    }
}
