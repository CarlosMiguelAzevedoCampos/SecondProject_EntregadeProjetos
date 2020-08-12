namespace CMA.ISMAI.Delivery.Payment.Domain.Interfaces
{
    public interface INotifyService
    {
        bool SendEmail(string body, string email);
    }
}
