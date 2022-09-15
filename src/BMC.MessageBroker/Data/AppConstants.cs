using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMC.MessageBroker.Data
{
    public class AppConstants
    {
        public const string AppName = "CamObserver.RadioTransceiver";
     
        public static string? DefaultPass { get; set; } = "123qweasd";
        public static string SQLConn = "";
        public const string GemLic = "EDWG-SKFA-D7J1-LDQ5";
        public static string RedisCon { set; get; }

        public static string GMapApiKey { get; set; }
        public static string BlobConn { get; set; }

       
        
    }
}
