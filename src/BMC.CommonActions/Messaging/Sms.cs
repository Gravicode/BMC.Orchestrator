using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMC.CommonActions.Messaging
{
    public class Sms
    {
        public static async Task<bool> SendSms(string ToNumber, string Message)

            => await Gravicode.Tools.SmsService.SendSms(Message, ToNumber);

    }
}
