using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;

namespace EasyBank.Services
{
    public class mailservice
    {
        private string From { get; set; }
        private string To { get; set; }
        private string Subject { get; set; }
        private string Message { get; set; }
        public mailservice(string from, string to, string subject, string message)
        {
            Send(from, to, subject, message);
            From = from;
            To = to;
            Subject = subject;
            Message = message;
        }

        public bool Send(string from, string to, string subject, string message)
        {
            From = from;
            To = to;
            Subject = subject;
            Message = message;
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    MailMessage message1 = new MailMessage();
                    message1.From = new MailAddress(this.From);
                    message1.To.Add(new MailAddress(this.To));
                    message1.Subject = this.Subject;
                    message1.Body = this.Message;
                    client.Credentials = new NetworkCredential("easybankbionic@gmail.com", "bionicc2");
                    client.EnableSsl = true;
                    client.Send(message1);
                }
            }
            catch(Exception ex){
                return false;
            }
            return true;
        }
    }
}