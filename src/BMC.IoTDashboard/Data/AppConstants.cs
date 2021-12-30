using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BMC.IoTDashboard.Data
{
    public class AppConstants
    {
        public const int FACE_WIDTH = 180;
        public const int FACE_HEIGHT = 135;
        public const string FACE_SUBSCRIPTION_KEY = "a068e60df8254cc5a187e3e8c644f316";
        public const string FACE_ENDPOINT = "https://southeastasia.api.cognitive.microsoft.com/";
        public static string ReportPJKBM = "";

        public static string SQLConn = "";
        public const string GemLic = "EDWG-SKFA-D7J1-LDQ5";
        public static string RedisCon { set; get; }

    }
}
