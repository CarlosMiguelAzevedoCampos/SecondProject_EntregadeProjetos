using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.CrossCutting.FileSaver
{
    public class FileSaverService : IFileSaverService
    {
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

            }
            return false;
        }
    }
}
