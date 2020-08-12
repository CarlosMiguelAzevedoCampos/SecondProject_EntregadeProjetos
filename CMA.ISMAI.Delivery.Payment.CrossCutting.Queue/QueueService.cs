using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;

namespace CMA.ISMAI.Delivery.Payment.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {
        public bool SendToQueue(DeliveryFileSystem delivery)
        {
            return true;
        }
    }
}
