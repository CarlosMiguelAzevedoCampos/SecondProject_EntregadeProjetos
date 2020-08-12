using CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations;
using Microsoft.AspNetCore.Http;
using System;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models
{
    public class CreateDeliveryWithFileCommand : DeliveryCommand
    {
        public IFormFile DeliveryFile { get; private set; }
        public CreateDeliveryWithFileCommand(string studentNumber, string courseName, string institutionName, string studentName, string studentEmail, IFormFile formFile, string cordenator, string title, string defenitionOfDelivery, string publicPDFVersionName, string privatePDFVersionName)
        {
            Id = Guid.NewGuid();
            CourseName = courseName;
            InstituteName = institutionName;
            StudentName = studentName;
            StudentNumber = studentNumber;
            DeliveryFile = formFile;
            DeliveryTime = DateTime.Now;
            StudentEmail = studentEmail;
            CordenatorName = cordenator;
            Title = title;
            DefenitionOfDelivery = defenitionOfDelivery;
            PublicPDFVersionName = publicPDFVersionName;
            PrivatePDFVersionName = privatePDFVersionName;
        }

        public override bool IsValid()
        {
            ValidationResult = new CreateDeliveryWithFileCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
