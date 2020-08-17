using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models
{
    public class GenerateJuryPageCommand : Command
    {
        public GenerateJuryPageCommand(string studentName, string studentNumber, string studentCourseName, string studentInstituteName, string filePath)
        {
            StudentName = studentName;
            StudentNumber = studentNumber;
            StudentCourseName = studentCourseName;
            StudentInstituteName = studentInstituteName;
            FilePath = filePath;
        }

        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string StudentCourseName { get; set; }
        public string StudentInstituteName { get; set; }
        public string FilePath { get; set; }
    }
}
