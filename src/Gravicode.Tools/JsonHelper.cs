using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravicode.Tools
{
    public class JsonHelper
    {
        public static bool IsValidJson(string strJson)
        {
            if (string.IsNullOrWhiteSpace(strJson)) { return false; }
            strJson = strJson.Trim();
            if ((strJson.StartsWith("{") && strJson.EndsWith("}")) || //For object
                (strJson.StartsWith("[") && strJson.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strJson);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
