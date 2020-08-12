using CMA.ISMAI.Delivery.API.Domain.Interfaces;

namespace CMA.ISMAI.Delivery.API.UI.IntegrationTests.IntegrationModels
{
    public class QueueService_IntegrationTests : IQueueService
    {
        public bool SendToQueue(Core.Model.Delivery delivery, string queueName)
        {
            return true;
        }
    }
}
