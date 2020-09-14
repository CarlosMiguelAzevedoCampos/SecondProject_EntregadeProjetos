using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model
{
    public class VerifyFilesCommand : Command
    {
        public VerifyFilesCommand(Guid id, string filepathExtract)
        {
            Id = id;
            FilePathExtract = filepathExtract;
        }
        public Guid Id { get; private set; }
        public string FilePathExtract { get; private set; }
    }
}
