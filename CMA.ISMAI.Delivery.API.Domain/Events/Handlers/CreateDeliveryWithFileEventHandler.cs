using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Events.Handlers
{
    public class CreateDeliveryWithFileEventHandler :
        INotificationHandler<CreateDeliveryWithFileEvent>
    {

        public Task Handle(CreateDeliveryWithFileEvent notification, CancellationToken cancellationToken)
        {
           // _emailSender.SendEmail(notification.Delivery);
            return Task.CompletedTask;
        }
    }
}
