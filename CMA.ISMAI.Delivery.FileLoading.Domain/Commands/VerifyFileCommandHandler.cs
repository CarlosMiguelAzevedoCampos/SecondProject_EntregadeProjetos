using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Commands
{
    public class VerifyFileCommandHandler : CommandHandler,
        IRequestHandler<VerifyFilesCommand, ValidationResult>
    {
        private readonly IFileVerifierService _fileVerifierService;

        public VerifyFileCommandHandler(IFileVerifierService fileVerifierService)
        {
            _fileVerifierService = fileVerifierService;
        }

        public Task<ValidationResult> Handle(VerifyFilesCommand request, CancellationToken cancellationToken)
        {
            if (_fileVerifierService.VerifyIfFilesAreCorrupted(request.FilePath))
                AddError("A corrupted file was found!");
            return Task.FromResult(ValidationResult);
        }
    }
}
