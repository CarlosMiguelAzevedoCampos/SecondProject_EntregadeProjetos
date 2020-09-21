namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IVerifyFilesExtensions
    {
        bool AreFilesInTheCorrectFormat(string filePath);
    }
}
