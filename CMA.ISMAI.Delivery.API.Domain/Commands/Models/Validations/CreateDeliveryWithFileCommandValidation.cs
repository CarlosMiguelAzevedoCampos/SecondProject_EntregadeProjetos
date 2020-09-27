namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations
{
    public class CreateDeliveryWithFileCommandValidation : DeliveryValidations<CreateDeliveryWithFileCommand>
    {
        public CreateDeliveryWithFileCommandValidation()
        {
            ValidateStudentName();
            ValidateStudentInstitution();
            ValidateStudentCourseName();
            ValidateStudentNumber();
            ValidateStudentEmail();
            ValidateCordenator();
            ValidateTitle();
            ValidateDefenition();
        }
    }
}
