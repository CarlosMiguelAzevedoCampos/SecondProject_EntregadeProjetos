using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events
{
    public class FilesIdentifiedEvent : Event
    {
        public FilesIdentifiedEvent(Guid id, string filePath, string studentEmail)
        {
            Id = id;
            FilePath = filePath;
            StudentEmail = studentEmail;
        }
        public Guid Id { get; private set; }
        public string FilePath { get; private set; }
        public string StudentEmail { get; private set; }
    }
}