using CMA.ISMAI.Delivery.API.Domain.Interfaces;

namespace CMA.ISMAI.Delivery.API.CrossCutting.Queue
{
    public class QueueService : IQueueService
    {
        public bool SendToQueue(Core.Model.Delivery delivery, string queueName)
        {
            return true;
        }
    }
}
