using FluentValidation;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations
{
    public abstract class DeliveryValidations<T> : AbstractValidator<T> where T : DeliveryCommand
    {
        protected void ValidateStudentName()
        {
            RuleFor(c => c.StudentName)
                .NotEmpty().WithMessage("Please ensure you have entered the Name")
                .Length(2, 150).WithMessage("The Name must have between 2 and 150 characters");
        }

        protected void ValidateStudentInstitution()
        {
            RuleFor(c => c.InstituteName)
                .NotEmpty().WithMessage("Please ensure you have entered the Institution")
                .Length(2, 150).WithMessage("The Institution Name must have between 2 and 150 characters");
        }

        protected void ValidateStudentCourseName()
        {
            RuleFor(c => c.CourseName)
                .NotEmpty().WithMessage("Please ensure you have entered the Course Name")
                .Length(2, 150).WithMessage("The Course Name must have between 2 and 150 characters");
        }
        protected void ValidateStudentNumber()
        {
            RuleFor(c => c.CourseName)
                .NotEmpty().WithMessage("Please ensure you have entered the Student Number")
                .Length(2, 150).WithMessage("The Student Number must have between 2 and 150 characters");
        }

        protected void ValidateStudentEmail()
        {
            RuleFor(c => c.StudentEmail)
                .NotEmpty().WithMessage("Please ensure you have entered the E-mail")
                .EmailAddress().WithMessage("E-mail is invalid.");
        }
        protected void ValidateCordenator()
        {
            RuleFor(c => c.CordenatorName)
                           .NotEmpty().WithMessage("Please ensure you have entered the Cordenator name")
                           .Length(2, 150).WithMessage("The Cordenator name must have between 2 and 150 characters");
        }

        protected void ValidateTitle()
        {
            RuleFor(c => c.Title)
                           .NotEmpty().WithMessage("Please ensure you have entered the Title")
                           .Length(2, 150).WithMessage("The Title must have between 2 and 150 characters");
        }

        protected void ValidateDefenition()
        {
            RuleFor(c => c.DefenitionOfDelivery)
                           .NotEmpty().WithMessage("Please ensure you have entered the Defenition Of Delivery")
                           .Length(2, 150).WithMessage("The Defenition Of Delivery must have between 2 and 150 characters");
        }
    }
}
