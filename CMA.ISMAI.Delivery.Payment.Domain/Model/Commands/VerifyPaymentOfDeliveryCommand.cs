using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.Payment.Domain.Model
{
    public class VerifyPaymentOfDeliveryCommand : Command
    {
        public VerifyPaymentOfDeliveryCommand(string studentNumber, string instituteName, string courseName, string filePath)
        {
            StudentNumber = studentNumber;
            InstituteName = instituteName;
            CourseName = courseName;
            FilePath = filePath;
        }

        public string StudentNumber { get; set; }
        public string InstituteName { get; set; }
        public string CourseName { get; set; }
        public string FilePath { get; set; }
    }
}
