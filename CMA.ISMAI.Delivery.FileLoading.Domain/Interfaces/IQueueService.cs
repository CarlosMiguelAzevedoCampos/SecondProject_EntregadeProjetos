using CMA.ISMAI.Core.Model;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IQueueService
    {
        bool SendToQueue(DeliveryFileSystem deliveryFileSystem, string queue);
    }
}
