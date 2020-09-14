using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events
{
    public class FilesVerifiedEvent : Event
    {
        public FilesVerifiedEvent(Guid id, string filepathExtract)
        {
            Id = id;
            FilePathExtract = filepathExtract;
        }
        public Guid Id { get; private set; }
        public string FilePathExtract { get; private set; }
    }
}
