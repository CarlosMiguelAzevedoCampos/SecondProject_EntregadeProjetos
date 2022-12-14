using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models
{
    public class CoverPageGeneratedEvent : Event
    {
        public CoverPageGeneratedEvent(string studentName, string cordenator, string deliveryContext, string title, string filePath)
        {
            StudentName = studentName;
            Cordenator = cordenator;
            DeliveryContext = deliveryContext;
            Title = title;
            FilePath = filePath;
        }

        public string StudentName { get; set; }
        public string Cordenator { get; set; }
        public string DeliveryContext { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
    }
}
