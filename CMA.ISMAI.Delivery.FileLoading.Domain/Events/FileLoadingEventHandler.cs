using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Events
{
    public class FileLoadingEventHandler : INotificationHandler<FileDownloadedEvent>,
        INotificationHandler<FilesIdentifiedEvent>, INotificationHandler<FilesVerifiedEvent>
    {
        private readonly IEventStoreService _eventStore;

        public FileLoadingEventHandler(IEventStoreService eventStore)
        {
            _eventStore = eventStore;
        }

        public Task Handle(FileDownloadedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }

        public Task Handle(FilesIdentifiedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }

        public Task Handle(FilesVerifiedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }
    }
}