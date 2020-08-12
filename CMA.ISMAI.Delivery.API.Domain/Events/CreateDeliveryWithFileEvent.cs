using CMA.ISMAI.Core.Model;
using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.API.Domain.Events
{
    public class CreateDeliveryWithFileEvent : Event
    {
        public DeliveryWithFile Delivery { get; private set; }
        public CreateDeliveryWithFileEvent(DeliveryWithFile delivery)
        {
            AggregateId = delivery.Id;
            Delivery = delivery;
        }
    }
}
