using CMA.ISMAI.Delivery.EventStore.Interface;
using CMA.ISMAI.Delivery.Payment.Domain.Model.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.Domain.Events
{
    public class PaymentEventHandler : INotificationHandler<PaymentCompletedEvent>
    {
        private readonly IEventStoreService _eventStore;

        public PaymentEventHandler(IEventStoreService eventStore)
        {
            _eventStore = eventStore;
        }


        public Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
        {
            _eventStore.SaveToEventStore(notification);
            return Task.CompletedTask;
        }
    }
}
