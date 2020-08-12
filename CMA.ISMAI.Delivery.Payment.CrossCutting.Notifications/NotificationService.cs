using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;

namespace CMA.ISMAI.Delivery.Payment.CrossCutting.Notifications
{
    public class NotificationService : INotifyService
    {
        public bool SendEmail(string body, string email)
        {
            return true;
        }
    }
}
