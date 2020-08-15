using CMA.ISMAI.Core.Interface;
using System;
using System.Net;
using System.Net.Mail;

namespace CMA.ISMAI.Core.Service
{
    public class NotificationService : INotificationService
    {
        public bool SendEmail(string email, string body)
        {

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("trelloismai@gmail.com");
                message.To.Add(new MailAddress(email));
                message.Subject = "ISMAI - Trello";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = body;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("trelloismai@gmail.com",
                    "trelloteste123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
