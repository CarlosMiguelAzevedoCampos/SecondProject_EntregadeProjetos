using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.Logging.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace CMA.ISMAI.Core.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ILoggingService _log;
        private readonly IConfiguration _config;
        public NotificationService(ILoggingService log)
        {
            _log = log;
            _config = new ConfigurationBuilder()
                                          .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                          .AddEnvironmentVariables()
                                          .Build();
        }

        public bool SendEmail(string email, string body)
        {

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(_config.GetSection("NotificationEmail:Email").Value);
                message.To.Add(new MailAddress(email));
                message.Subject = "Delivery System";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = body;
                smtp.Port = Convert.ToInt32(_config.GetSection("NotificationEmail:Port").Value);
                smtp.Host = _config.GetSection("NotificationEmail:Host").Value;  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_config.GetSection("NotificationEmail:Email").Value,
                     _config.GetSection("NotificationEmail:Password").Value);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }
    }
}
