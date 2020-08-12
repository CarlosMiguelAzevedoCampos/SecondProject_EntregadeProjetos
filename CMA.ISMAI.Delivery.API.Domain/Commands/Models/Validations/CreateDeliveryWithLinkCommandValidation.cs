using FluentValidation;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations
{
    public class CreateDeliveryWithLinkCommandValidation : DeliveryValidations<CreateDeliveryWithLinkCommand>
    {
        public CreateDeliveryWithLinkCommandValidation()
        {
            ValidateStudentCourseName();
            ValidateStudentInstitution();
            ValidateStudentName();
            ValidateStudentNumber();
            ValidateUrl();
            ValidateCordenator();
            ValidateTitle();
            ValidateDefenition();
        }

        protected void ValidateUrl()
        {
            RuleFor(c => c.LinkFile)
                .NotEmpty().WithMessage("Please ensure you have entered the File Url");
        }
    }
}
