using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.Payment.Domain.Model
{
    public class VerifyPaymentOfDeliveryCommand : Command
    {
        public VerifyPaymentOfDeliveryCommand(string studentNumber, string instituteName, string courseName)
        {
            StudentNumber = studentNumber;
            InstituteName = instituteName;
            CourseName = courseName;
        }

        public string StudentNumber { get; set; }
        public string InstituteName { get; set; }
        public string CourseName { get; set; }
    }
}
