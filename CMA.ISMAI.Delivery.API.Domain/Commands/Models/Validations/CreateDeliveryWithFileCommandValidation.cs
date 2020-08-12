namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations
{
    public class CreateDeliveryWithFileCommandValidation : DeliveryValidations<CreateDeliveryWithFileCommand>
    {
        public CreateDeliveryWithFileCommandValidation()
        {
            ValidateStudentCourseName();
            ValidateStudentInstitution();
            ValidateStudentName();
            ValidateStudentNumber();
            ValidateCordenator();
            ValidateTitle();
            ValidateDefenition();
        }
    }
}
