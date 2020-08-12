namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IPDFVerifierService
    {
        bool ArePdfFilesOk(string filePath);
    }
}
