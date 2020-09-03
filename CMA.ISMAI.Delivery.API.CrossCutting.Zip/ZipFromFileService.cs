using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CMA.ISMAI.Delivery.API.CrossCutting.Zip
{
    public class ZipFromFileService : IZipFileService
    {

        private readonly ILoggingService _log;

        public ZipFromFileService(ILoggingService log)
        {
            _log = log;
        }

        public bool DoesTheZipFileContainsaPDF(IFormFile deliveryFile)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    deliveryFile.CopyTo(ms);
                    var zipArchive = new ZipArchive(ms);
                    return zipArchive.Entries.Where(x => Path.GetExtension(x.Name).ToLower() == ".pdf").Count() > 0;
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        public bool DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(IFormFile deliveryFile, string publicVersion, string privateVersion)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    deliveryFile.CopyTo(ms);
                    var zipArchive = new ZipArchive(ms);
                    return zipArchive.Entries.Where(x => (x.Name.Contains(publicVersion) && (Path.GetExtension(x.Name).ToLower() == ".pdf"))
                                   || (x.Name.Contains(privateVersion) && (Path.GetExtension(x.Name).ToLower() == ".pdf"))).Count() > 0;
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        public bool DoesZipFileContainsFiles(IFormFile file)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    ZipArchive archive = new ZipArchive(ms);
                    return archive.Entries.Count > 0;
                }

            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }
    }
}
