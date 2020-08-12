namespace CMA.ISMAI.Delivery.API.Domain.Interfaces
{
    public interface IQueueService
    {
        bool SendToQueue(Core.Model.Delivery delivery, string queueName);
    }
}
