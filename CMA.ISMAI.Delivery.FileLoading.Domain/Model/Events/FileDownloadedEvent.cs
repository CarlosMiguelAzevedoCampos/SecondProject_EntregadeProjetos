using CMA.ISMAI.Core.Model;
using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events
{
    public class FileDownloadedEvent : Event
    {
        public FileDownloadedEvent(DeliveryFileSystem delivery)
        {
            Delivery = delivery;
        }

        public DeliveryFileSystem Delivery { get; private set; }

    }
}
