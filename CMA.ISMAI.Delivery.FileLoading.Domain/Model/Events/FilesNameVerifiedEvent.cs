using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events
{
    public class FilesNameVerifiedEvent : Event
    {
        public Guid id { get; private set; }
        public string filePath { get; private set; }
        public string filePathExtract { get; private set; }
        public string privateFile { get; private set; }
        public string publicFile { get; private set; }

        public FilesNameVerifiedEvent(Guid id, string filePath, string filePathExtract, string privateFile, string publicFile)
        {
            this.id = id;
            this.filePath = filePath;
            this.filePathExtract = filePathExtract;
            this.privateFile = privateFile;
            this.publicFile = publicFile;
        }
    }
}