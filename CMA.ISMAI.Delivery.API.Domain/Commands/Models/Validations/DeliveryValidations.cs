using FluentValidation;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models.Validations
{
    public abstract class DeliveryValidations<T> : AbstractValidator<T> where T : DeliveryCommand
    {
        protected void ValidateStudentName()
        {
            RuleFor(c => c.StudentName)
                .NotEmpty().WithMessage("Porfavor, insira o seu nome")
                .Length(2, 150).WithMessage("O nome deve ter entre 2 a 150 caracteres!");
        }

        protected void ValidateStudentInstitution()
        {
            RuleFor(c => c.InstituteName)
                .NotEmpty().WithMessage("Porfavor, insira a Instituição")
                .Length(2, 10).WithMessage("O Nome da Instituição deve ter entre 2 a 10 caracteres");
        }

        protected void ValidateStudentCourseName()
        {
            RuleFor(c => c.CourseName)
                .NotEmpty().WithMessage("Porfavor, insira o seu curso")
                .Length(2, 20).WithMessage("O nome do curso deve ter entre 2 a 20 caracteres");
        }
        protected void ValidateStudentNumber()
        {
            RuleFor(c => c.StudentNumber)
                .NotEmpty().WithMessage("Porfavor, insira o seu número de aluno")
                .Length(2, 10).WithMessage("O Número de aluno deve ter entre 2 a 10 caracteres");
        }

        protected void ValidateStudentEmail()
        {
            RuleFor(c => c.StudentEmail)
                .NotEmpty().WithMessage("Porfavor, insira o seu e-mail")
                .EmailAddress().WithMessage("E-mail não é valido");
        }
        protected void ValidateCordenator()
        {
            RuleFor(c => c.CordenatorName)
                .NotEmpty().WithMessage("Porfavor, insira o seu coordenador!")
                .Length(2, 20).WithMessage("O nome do Coordenador deve ter entre 2 a 20 caracteres");
        }

        protected void ValidateTitle()
        {
            RuleFor(c => c.Title)
                           .NotEmpty().WithMessage("Porfavor, insira o seu Título")
                .Length(2, 100).WithMessage("O título do projeto deve ter entre 2 a 100 caracteres");
        }

        protected void ValidateDefenition()
        {
            RuleFor(c => c.DefenitionOfDelivery)
                           .NotEmpty().WithMessage("Porfavor, insira a sua defenição de entrega")
                .Length(2, 20).WithMessage("A definição deve ter entre 2 a 20 caracteres");
        }
    }
}
