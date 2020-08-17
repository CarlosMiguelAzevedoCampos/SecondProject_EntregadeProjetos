using CMA.ISMAI.Delivery.EventStore.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Events.Handlers
{
    public class CreateDeliveryWithFileEventHandler :
        INotificationHandler<CreateDeliveryWithFileEvent>
    {
        private readonly IEventStoreService _eventStore;

        public CreateDeliveryWithFileEventHandler(IEventStoreService eventStore)
        {
            _eventStore = eventStore;
        }

        public Task Handle(CreateDeliveryWithFileEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }
    }
}
