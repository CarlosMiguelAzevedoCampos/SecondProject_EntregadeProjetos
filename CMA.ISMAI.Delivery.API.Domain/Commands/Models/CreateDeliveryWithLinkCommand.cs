using CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations;
using System;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models
{
    public class CreateDeliveryWithLinkCommand : DeliveryCommand
    {
        public string LinkFile { get; private set; }
        public CreateDeliveryWithLinkCommand(string studentNumber, string courseName, string institutionName, string studentName, string studentEmail, string linkFile, string cordenatorName, string title, string defenitionOfDelivery, string privatePdfName, string publicPdfName)
        {
            Id = Guid.NewGuid();
            CourseName = courseName;
            InstituteName = institutionName;
            StudentName = studentName;
            StudentNumber = studentNumber;
            LinkFile = linkFile;
            DeliveryTime = DateTime.Now;
            StudentEmail = studentEmail;
            CordenatorName = cordenatorName;
            Title = title;
            DefenitionOfDelivery = defenitionOfDelivery;
            PublicPDFVersionName = publicPdfName;
            PrivatePDFVersionName = privatePdfName;
        }

        public override bool IsValid()
        {
            ValidationResult = new CreateDeliveryWithLinkCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
