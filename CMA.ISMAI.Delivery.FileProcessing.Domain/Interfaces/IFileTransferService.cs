namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces
{
    public interface IFileTransferService
    {
        bool TransferFile(string filePath, string drivePath);
    }
}
