using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.EventStore.Interface
{
    public interface IEventStoreService
    {
        void SaveToEventStore(Event @event);
    }
}
