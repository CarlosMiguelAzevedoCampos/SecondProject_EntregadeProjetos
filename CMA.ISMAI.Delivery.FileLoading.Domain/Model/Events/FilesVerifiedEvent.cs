using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events
{
    public class FilesVerifiedEvent : Event
    {
        public FilesVerifiedEvent(Guid id, string filePath, string filepathExtract)
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
