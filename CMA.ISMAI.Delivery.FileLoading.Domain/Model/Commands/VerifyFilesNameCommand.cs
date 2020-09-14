using NetDevPack.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model.Commands
{
    public class VerifyFilesNameCommand : Command
    {
        public VerifyFilesNameCommand(Guid id, string filePath, string filepathExtract, string publicFile, string privateFile)
        {
            Id = id;
            FilePath = filePath;
            FilePathExtract = filepathExtract;
            PublicFile = publicFile;
            PrivateFile = privateFile;
        }
        public Guid Id { get; private set; }
        public string FilePath { get; private set; }
        public string FilePathExtract { get; private set; }
        public string PrivateFile { get; private set; }
        public string PublicFile { get; private set; }
    }
}
