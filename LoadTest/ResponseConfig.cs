using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTest
{
    public class ResponseConfig
    {
        public int ExpectedStatusCode { get; set; }
        public string ExpectedResponseContains { get; set; }
    }
}
