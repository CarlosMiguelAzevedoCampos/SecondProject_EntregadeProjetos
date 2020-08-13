using CMA.ISMAI.Core.Model;
using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.API.Domain.Events
{
    public class CreateDeliveryWithFileEvent : Event
    {
        public DeliveryFileSystem Delivery { get; private set; }
        public CreateDeliveryWithFileEvent(DeliveryFileSystem delivery)
        {
            AggregateId = delivery.Id;
            Delivery = delivery;
        }
    }
}
