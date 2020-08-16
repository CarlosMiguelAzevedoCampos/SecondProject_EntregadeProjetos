using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.CrossCutting.FileSaver
{
    public class FileSaverService : IFileSaverService
    {
        private readonly ILoggingService _log;

        public FileSaverService(ILoggingService log)
        {
            _log = log;
        }

        public async Task<bool> DownloadFile(IFormFile file, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }
    }
}
