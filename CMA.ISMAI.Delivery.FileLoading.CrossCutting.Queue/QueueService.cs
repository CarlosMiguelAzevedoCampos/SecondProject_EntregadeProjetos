using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {

        public bool SendToQueue(DeliveryFileSystem deliveryFileSystem, string queue)
        {
            return true;
        }
    }
}
