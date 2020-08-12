namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces
{
    public interface IGenerateWaterMarkService
    {
        bool AddWaterMark(string path, string publicFile, string privateFile);
    }
}