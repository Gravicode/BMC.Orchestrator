using Gravicode.ExpressionEvaluator;
using Newtonsoft.Json;
using System.Dynamic;

namespace BMC.StreamProcessor
{
    public class UrlAction : AlertAction
    {
        public string Url { get; set; }
        public UrlAction(string url)
        {
            this.Url = url;
        }

        async Task<bool> AlertAction.DoAction()
        {
            try
            {
                var res = await BMC.CommonActions.Web.Curl.GetString(this.Url);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
    public class SmsAction : AlertAction
    {
        public string NoHp { get; set; }
        public string Message { get; set; }
        public SmsAction(string nohp, string message)
        {
            this.NoHp = nohp;
            this.Message = message;
        }

        async Task<bool> AlertAction.DoAction()
        {
            return await BMC.CommonActions.Messaging.Sms.SendSms(NoHp,Message);
        }
    }
    public class EmailAction : AlertAction
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public EmailAction(string subject,string body,string toemail)
        {
            this.Subject = subject;
            this.Body = body;
            this.ToEmail = toemail;
        }

        async Task<bool> AlertAction.DoAction()
        {
            return await BMC.CommonActions.Messaging.Email.SendEmail(ToEmail, Subject, Body);
        }
    }
    public interface AlertAction
    {
        Task<bool> DoAction();
    }
    public class AlertEventArgs : EventArgs
    {
        public string? AlertName { get; set; }
        public object? DataObject { get; set; }
    }
    public class StreamPipe
    {
        public List<AlertAction> AlertActions=new();
        List<dynamic> dataCache;
        EvaluatorEngine engine;
        public Dictionary<string, string> AlertList;
        public event AlertEventHandler OnAlertTriggered;
        public delegate void AlertEventHandler(object sender, AlertEventArgs e);
        public bool UseCache { get; set; } = true;
        public TimeSpan CacheExpiration { get; set; } = new TimeSpan(0,30,0);
        public StreamPipe()
        {
            dataCache = new List<dynamic>();
            engine = new EvaluatorEngine();
            AlertList = new Dictionary<string, string>();   
        }
        public bool RemoveAlert(string Name)
        {
            if (AlertList.ContainsKey(Name))
            {
                AlertList.Remove(Name);
                return true;
            }
            Console.WriteLine($"can't find alert name = {Name}");
            return false;
        }
        public bool AddAlert(string Name,string Condition)
        {
            if (AlertList.ContainsKey(Name))
            {
                Console.WriteLine("alert name is already exists");
                return false;
            }
            AlertList[Name] = Condition;
            return true;
        }

        /// <summary>
        /// Process json data to check alert condition
        /// </summary>
        /// <param name="DataJson"></param>
        /// <returns></returns>
        public bool DoAlertFiltering(string DataJson)
        {
            bool isValid = false;
            var item = ConvertToKV(DataJson);
            if (item is not null && AlertList.Count > 0)
            {
                isValid = true; 
                foreach (var alert in AlertList)
                {
                    var condition = replaceStatement(alert.Value);
                    var hasil = engine.EvaluateLogic(condition);
                    if (hasil)
                    {
                        OnAlertTriggered?.Invoke(this, new AlertEventArgs() { AlertName = alert.Key, DataObject = DataJson });
                    }
                }
            }
            else return false;
            return isValid;
            string replaceStatement(string AlertCondition)
            {
                if (string.IsNullOrEmpty(AlertCondition)) return null;
                //replace all vars with fixed value
                foreach (var key in item)
                {
                    if (AlertCondition.IndexOf(key.Key, 0, StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        AlertCondition = AlertCondition.Replace(key.Key,key.Value.ToString(),StringComparison.InvariantCultureIgnoreCase);
                    }
                }
                return AlertCondition; 
            }
        }

        void AddToCache(string DataJson)
        {
            var item = ConvertToKV(DataJson);
            if (item is not null)
            {
                //add item to cache, add time attribute
                var obj = SchemaConverter.ConvertToExpandoObjects(item);
                obj.SysDateReceived = DateTime.Now;
                dataCache.Add(obj);
            }
        }

        /// <summary>
        /// Convert json data to key value pair
        /// </summary>
        /// <param name="DataJson"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, object>> ConvertToKV(string DataJson)
        {
            if (string.IsNullOrEmpty(DataJson)) return null;
            var list = new List<KeyValuePair<string, object>>();
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(DataJson);
            if (obj != null) {
                foreach (KeyValuePair<string, object> item in obj)
                {
                    list.Add(item);
                } 
            }
            return list;

        }

        void removeExpiredCache()
        {
            var removedItems = new List<dynamic>();
            foreach(var item in dataCache)
            {

                var expDate = DateTime.Now - CacheExpiration;
                if (item.SysDateReceived < expDate)
                {
                    removedItems.Add(item);
                }
            }
            foreach(var item in removedItems)
            {
                dataCache.Remove(item); 
            }
        }

        public void ProcessData(string DataJson)
        {
            //add to cache
            if (UseCache)
                AddToCache(DataJson);

            //alert filtering
            var result = DoAlertFiltering(DataJson);

            //query stream in cache

            //action routing

            if (UseCache)
                removeExpiredCache();
        }
    }
}