namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IFileVerifierService
    {
        bool VerifyIfFilesAreCorrupted(string filePath);
    }
}
