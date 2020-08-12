using CMA.ISMAI.Core.Model;
using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.API.Domain.Events
{
    public class CreateDeliveryWithLinkEvent : Event
    {
        public DeliveryWithLink Delivery { get; private set; }
        public CreateDeliveryWithLinkEvent(DeliveryWithLink delivery)
        {
            AggregateId = delivery.Id;
            Delivery = delivery;
        }
    }
}
