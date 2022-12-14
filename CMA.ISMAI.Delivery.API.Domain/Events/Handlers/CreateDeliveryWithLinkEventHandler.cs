using CMA.ISMAI.Delivery.EventStore.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Events.Handlers
{
    public class CreateDeliveryWithLinkEventHandler :
           INotificationHandler<CreateDeliveryWithLinkEvent>
    {
        private readonly IEventStoreService _eventStore;

        public CreateDeliveryWithLinkEventHandler(IEventStoreService eventStore)
        {
            _eventStore = eventStore;
        }

        public Task Handle(CreateDeliveryWithLinkEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }
    }
}
