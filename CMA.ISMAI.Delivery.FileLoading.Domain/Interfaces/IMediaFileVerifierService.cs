namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IMediaFileVerifierService
    {
        bool AreMediaFilesOk(string filePath);
    }
}
