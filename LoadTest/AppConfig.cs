using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTest
{
    public class AppConfig
    {
        public int RequestDurationInMin { get; set; }
        public int RequestIntervalMilliseconds { get; set; }
        public int RequestCount { get; set; }
    }
}
