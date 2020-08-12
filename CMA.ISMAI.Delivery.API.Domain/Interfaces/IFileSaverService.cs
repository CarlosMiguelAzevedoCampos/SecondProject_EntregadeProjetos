using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Interfaces
{
    public interface IFileSaverService
    {
        Task<bool> DownloadFile(IFormFile file, string filePath);
    }
}
