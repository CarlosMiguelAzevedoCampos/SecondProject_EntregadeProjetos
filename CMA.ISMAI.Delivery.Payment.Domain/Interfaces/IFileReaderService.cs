namespace CMA.ISMAI.Delivery.Payment.Domain.Interfaces
{
    public interface IFileReaderService
    {
        bool PaymentHasBeenDone(string studentNumber, string courseName, string institutionName, string filePath);
    }
}