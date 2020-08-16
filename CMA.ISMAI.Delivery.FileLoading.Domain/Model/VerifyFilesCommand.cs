using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model
{
    public class VerifyFilesCommand : Command
    {
        public VerifyFilesCommand(Guid id, string filePath, string filepathExtract)
        {
            Id = id;
            FilePath = filePath;
            FilePathExtract = filepathExtract;
        }
        public Guid Id { get; private set; }
        public string FilePath { get; private set; }
        public string FilePathExtract { get; private set; }
    }
}
