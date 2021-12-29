using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMC.CommonActions.Web
{
    public class Curl
    {
        static HttpClient _client;
        static HttpClient client
        {
            get
            {
                if (_client == null) _client = new HttpClient();
                return _client;
            }
        }

        public static async Task<byte[]> GetBytes(string Url)
        {
            var result = await client.GetByteArrayAsync(Url);
            return result;
        }

        public static async Task<string> GetString(string Url)
        {
            var result = await client.GetStringAsync(Url);
            return result;
        }

        public static async Task<T> GetObject<T>(string Url)
        {
            var result = await client.GetStringAsync(Url);
            var obj = JsonConvert.DeserializeObject<T>(result);
            return obj;
        }

        public static async Task<bool> PostString(string Url, string Data, CancellationToken token = default)
        {
            var result = await client.PostAsync(Url, new StringContent(Data, Encoding.UTF8, "application/json"), token);
            return result.IsSuccessStatusCode;
        }

        public static async Task<bool> PostBytes(string Url, byte[] Data, CancellationToken token = default)
        {
            var result = await client.PostAsync(Url, new ByteArrayContent(Data), token);
            return result.IsSuccessStatusCode;
        }
        public static async Task<(bool, string)> PostStringWithReturn(string Url, string Data, CancellationToken token = default)
        {
            try
            {
                var result = await client.PostAsync(Url, new StringContent(Data, Encoding.UTF8, "application/json"), token);
                var requestResult = result.IsSuccessStatusCode;
                var stringResult = await result.Content.ReadAsStringAsync();
                return (requestResult, stringResult);

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

        }

        public static async Task<(bool, string)> PostBytesWithReturn(string Url, byte[] Data, CancellationToken token = default)
        {
            try
            {
                var result = await client.PostAsync(Url, new ByteArrayContent(Data), token);
                var requestResult = result.IsSuccessStatusCode;
                var stringResult = await result.Content.ReadAsStringAsync();
                return (requestResult, stringResult);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

}

