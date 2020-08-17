using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Events
{
    public class FileProcessingEventHandler : INotificationHandler<CoverPageGeneratedEvent>,
        INotificationHandler<JuryPageGeneretedEvent>,
        INotificationHandler<WaterMarkGeneratedEvent>
    {
        private readonly IEventStoreService _eventStore;

        public FileProcessingEventHandler(IEventStoreService eventStore)
        {
            _eventStore = eventStore;
        }

        public Task Handle(CoverPageGeneratedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }

        public Task Handle(WaterMarkGeneratedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }

        public Task Handle(JuryPageGeneretedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }
    }
}
