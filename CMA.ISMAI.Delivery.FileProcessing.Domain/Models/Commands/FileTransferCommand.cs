using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Commands
{
    public class FileTransferCommand : Command
    {
        public FileTransferCommand(string filePath, string oneDrivePath, string studentEmail)
        {
            FilePath = filePath;
            OneDrivePath = oneDrivePath;
            StudentEmail = studentEmail;
        }

        public string FilePath { get; set; }
        public string OneDrivePath { get; set; }
        public string StudentEmail { get; set; }
    }
}
