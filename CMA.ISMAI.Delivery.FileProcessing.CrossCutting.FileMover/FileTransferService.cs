using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using System;
using System.IO;
using System.IO.Compression;

namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileMover
{
    public class FileTransferService : IFileTransferService
    {
        private readonly ILoggingService _log;

        public FileTransferService(ILoggingService log)
        {
            _log = log;
        }

        public bool TransferFile(string filePath, string drivePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                ZipFile.CreateFromDirectory(filePath, string.Format("{0}\\{1}.zip", drivePath, fileName));   
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
