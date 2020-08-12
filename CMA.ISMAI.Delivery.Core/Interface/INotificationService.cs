namespace CMA.ISMAI.Core.Interface
{
    public interface INotificationService
    {
        bool SendEmail(string email, string body);
    }
}
