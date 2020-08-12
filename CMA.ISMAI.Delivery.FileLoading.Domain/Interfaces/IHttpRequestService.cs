using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IHttpRequestService
    {
        bool DownloadFileToHost(string pathToSave, string fileUrl);
    }
}
