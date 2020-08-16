using CMA.ISMAI.Core.Model;

namespace CMA.ISMAI.Delivery.Payment.Domain.Interfaces
{
    public interface IQueueService
    {
        bool SendToQueue(DeliveryFileSystem deliveryFileSystem, string queue);
    }
}
