using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMC.CommonActions.Messaging
{
    public class Email
    {
        public static async Task<bool> SendEmail(string ToEmail, string Subject, string Body)
        =>
           await Gravicode.Tools.MailService.SendEmail(Subject, Body, ToEmail, true);

    }
}
