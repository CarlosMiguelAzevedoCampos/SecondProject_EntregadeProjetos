using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models
{
    public class GenerateCoverPageCommand : Command
    {
        public GenerateCoverPageCommand(string studentName, string cordenator, string deliveryContext, string title, string filePath, string coverImagePath)
        {
            StudentName = studentName;
            Cordenator = cordenator;
            DeliveryContext = deliveryContext;
            Title = title;
            FilePath = filePath;
            CoverImagePath = coverImagePath;
        }

        public string StudentName { get; set; }
        public string Cordenator { get; set; }
        public string DeliveryContext { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string CoverImagePath { get; set; }
    }
}
