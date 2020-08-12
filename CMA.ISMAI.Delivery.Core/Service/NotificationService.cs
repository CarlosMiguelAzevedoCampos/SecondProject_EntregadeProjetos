using CMA.ISMAI.Core.Interface;

namespace CMA.ISMAI.Core.Service
{
    public class NotificationService : INotificationService
    {
        public bool SendEmail(string email, string body)
        {
            return true;
        }
    }
}
