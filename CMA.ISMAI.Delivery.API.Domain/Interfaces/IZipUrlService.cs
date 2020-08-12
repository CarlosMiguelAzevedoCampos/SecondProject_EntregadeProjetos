using System.IO;

namespace CMA.ISMAI.Delivery.API.Domain.Interfaces
{
    public interface IZipUrlService
    {
        bool DoesZipFileContainsFiles(MemoryStream fileList);
        bool DoesTheZipFileContainsaPDF(MemoryStream deliveryFile);
        bool DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(MemoryStream deliveryFile, string publicVersion, string privateVersion);
    }
}
