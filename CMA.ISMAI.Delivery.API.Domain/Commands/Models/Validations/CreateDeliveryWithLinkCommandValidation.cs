using FluentValidation;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations
{
    public class CreateDeliveryWithLinkCommandValidation : DeliveryValidations<CreateDeliveryWithLinkCommand>
    {
        public CreateDeliveryWithLinkCommandValidation()
        {
            ValidateStudentName();
            ValidateStudentInstitution();
            ValidateStudentCourseName();
            ValidateStudentNumber();
            ValidateStudentEmail();
            ValidateCordenator();
            ValidateTitle();
            ValidateDefenition();
            ValidateUrl();
        }

        protected void ValidateUrl()
        {
            RuleFor(c => c.LinkFile)
                .NotEmpty().WithMessage("Porfavor, entregue preencha o campo Link com o Link da entrega.");
        }
    }
}
