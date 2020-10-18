using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Commands
{
    public class FileTransferCommand : Command
    {
        public FileTransferCommand(string filePath, string oneDrivePath, string studentEmail, string studentNumber)
        {
            FilePath = filePath;
            OneDrivePath = oneDrivePath;
            StudentEmail = studentEmail;
            StudentNumber = studentNumber;
        }

        public string FilePath { get; set; }
        public string OneDrivePath { get; set; }
        public string StudentEmail { get; set; }
        public string StudentNumber { get; set; }
    }
}
