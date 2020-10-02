namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces
{
    public interface ICoverPageService
    {
        bool AddCoverPage(string basepath, string title, string studentName, string cordenatorName, string contextOfDelivery, string imagePath);
    }
}
