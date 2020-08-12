using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileIdentifier
{
    public class FileIdentifierService : IFileIdentifierService
    {
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

            }
            return new Dictionary<string, Guid>();
        }

        private string[] ReturnAllFilesInADirectory(string filepath)
        {
            return Directory.GetFiles(filepath, "*.*", SearchOption.AllDirectories);
        }
    }
}
