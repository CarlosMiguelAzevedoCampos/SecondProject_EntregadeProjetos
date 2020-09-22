namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IFileVerifierService
    {
        bool VerifyFilesConditions(string filePath);

        bool UnzipFiles(string filePath, string extractPath);
        bool VerifyIfPublicAndPrivateFilesExist(string filePathExtract, string privateFile, string publicFile);
    }
}
