using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Events
{
    public class FileTransferCompletedEvent : Event
    {
        public FileTransferCompletedEvent(string studentEmail, string filePath, string oneDrivePath)
        {
            StudentEmail = studentEmail;
            FilePath = filePath;
            OneDrivePath = oneDrivePath;
        }

        public string StudentEmail { get; set; }
        public string FilePath { get; set; }
        public string OneDrivePath { get; set; }
    }
}
