using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Events.Handlers
{
    public class CreateDeliveryWithLinkEventHandler :
           INotificationHandler<CreateDeliveryWithLinkEvent>
    {
        public Task Handle(CreateDeliveryWithLinkEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
