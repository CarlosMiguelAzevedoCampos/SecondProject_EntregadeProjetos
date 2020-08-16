using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CMA.ISMAI.Delivery.API.CrossCutting.Zip
{
    public class ZipFromFileService : IZipFileService
    {
        public bool DoesTheZipFileContainsaPDF(IFormFile deliveryFile)
        {
            try
            {
                var zipArchive = new ZipArchive(deliveryFile.OpenReadStream());

                return zipArchive.Entries.Where(x => Path.GetExtension(x.Name).ToLower() == ".pdf").Count() > 0;
            }
            catch (Exception ex)
            {
                // Do logging stuff
            }
            return false;
        }

        public bool DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(IFormFile deliveryFile, string publicVersion, string privateVersion)
        {
            try
            {
                var zipArchive = new ZipArchive(deliveryFile.OpenReadStream());

                return zipArchive.Entries.Where(x => (x.Name.Contains(publicVersion) && (Path.GetExtension(x.Name).ToLower() == ".pdf"))
                                   || (x.Name.Contains(privateVersion) && (Path.GetExtension(x.Name).ToLower() == ".pdf"))).Count() > 0;
            }
            catch (Exception ex)
            {
                // Do logging stuff
            }
            return false;
        }

        public bool DoesZipFileContainsFiles(IFormFile file)
        {
            try
            {
                ZipArchive archive = new ZipArchive(file.OpenReadStream());
                return archive.Entries.Count > 0;
            }
            catch (Exception ex)
            {
                // Do logging stuff
            }
            return false;
        }
    }
}
