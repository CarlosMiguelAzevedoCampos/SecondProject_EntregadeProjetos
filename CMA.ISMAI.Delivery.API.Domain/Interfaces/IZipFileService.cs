using Microsoft.AspNetCore.Http;

namespace CMA.ISMAI.Delivery.API.Domain.Interfaces
{
    public interface IZipFileService
    {
        bool DoesZipFileContainsFiles(IFormFile fileList);
        bool DoesTheZipFileContainsaPDF(IFormFile deliveryFile);
        bool DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(IFormFile deliveryFile, string publicVersion, string privateVersion);
    }
}
