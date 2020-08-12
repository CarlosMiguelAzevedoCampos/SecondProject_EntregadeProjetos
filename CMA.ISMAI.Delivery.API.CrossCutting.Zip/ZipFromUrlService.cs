using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CMA.ISMAI.Delivery.API.CrossCutting.Zip
{
    public class ZipFromUrlService : IZipUrlService
    {
        public bool DoesTheZipFileContainsaPDF(MemoryStream deliveryFile)
        {
            try
            {
                ZipArchive archive = new ZipArchive(deliveryFile);
                return archive.Entries.Where(x => Path.GetExtension(x.FullName).ToLower() == ".pdf").Count() > 0;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(MemoryStream deliveryFile, string publicVersion, string privateVersion)
        {
            try
            {
                ZipArchive archive = new ZipArchive(deliveryFile);
                return archive.Entries.Where(x => (x.FullName.Contains(publicVersion) && (Path.GetExtension(x.FullName).ToLower() == ".pdf"))
                                    || (x.FullName.Contains(privateVersion) && (Path.GetExtension(x.FullName).ToLower() == ".pdf"))).Count() > 0;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool DoesZipFileContainsFiles(MemoryStream deliveryFile)
        {
            try
            {
                ZipArchive archive = new ZipArchive(deliveryFile);
                return archive.Entries.Count > 0;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
