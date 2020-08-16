using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier
{
    public class FileIdentifierService : IFileIdentifierService
    {
        private readonly ILoggingService _log;

        public FileIdentifierService(ILoggingService log)
        {
            _log = log;
        }
        public Dictionary<string, Guid> GenerateFileIdentifier(string filepath)
        {
            try
            {
                string[] files = ReturnAllFilesInADirectory(filepath);
                Dictionary<string, Guid> filesInformation = new Dictionary<string, Guid>();
                foreach (var item in files)
                {
                    FileInfo info = new FileInfo(item);
                    filesInformation.Add(info.FullName, Guid.NewGuid());
                }
                return filesInformation;
            }
            catch(Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return new Dictionary<string, Guid>();
        }

        private string[] ReturnAllFilesInADirectory(string filepath)
        {
            return Directory.GetFiles(filepath, "*.*", SearchOption.AllDirectories);
        }
    }
}
