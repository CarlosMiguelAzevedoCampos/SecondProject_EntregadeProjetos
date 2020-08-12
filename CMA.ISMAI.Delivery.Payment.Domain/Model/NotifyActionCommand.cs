using NetDevPack.Messaging;

namespace CMA.ISMAI.Delivery.Payment.Domain.Model
{
    public class NotifyActionCommand : Command
    {
        public NotifyActionCommand(string studentNumber, string instituteName, string courseName, string email, string body)
        {
            StudentNumber = studentNumber;
            InstituteName = instituteName;
            CourseName = courseName;
            Email = email;
            Body = body;
        }

        public string StudentNumber { get; set; }
        public string InstituteName { get; set; }
        public string CourseName { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }
}