using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Events
{
    public class FileProcessingEventHandler : INotificationHandler<CoverPageGeneratedEvent>,
        INotificationHandler<JuryPageGeneretedEvent>,
        INotificationHandler<WaterMarkGeneratedEvent>,
        INotificationHandler<FileTransferCompletedEvent>
    {
        private readonly IEventStoreService _eventStore;
        private readonly INotificationService _notificationService;

        public FileProcessingEventHandler(IEventStoreService eventStore, INotificationService notificationService)
        {
            _eventStore = eventStore;
            _notificationService = notificationService;
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

        public Task Handle(FileTransferCompletedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            _notificationService.SendEmail(notification.StudentEmail, "Hey!, <br/> <br/> You're delivery is already on the institution OneDrive! <br/><br/> Thanks,<br/> <br/> Delivery System");
            return Task.CompletedTask;
        }
    }
}
