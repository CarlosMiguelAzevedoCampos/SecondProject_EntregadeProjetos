using CMA.ISMAI.Core.Model;

namespace CMA.ISMAI.Delivery.API.Domain.Interfaces
{ 
    public interface IQueueService
    {
        bool SendToQueue(Core.Model.Delivery deliveryFileSystem, string queue);
    }
}