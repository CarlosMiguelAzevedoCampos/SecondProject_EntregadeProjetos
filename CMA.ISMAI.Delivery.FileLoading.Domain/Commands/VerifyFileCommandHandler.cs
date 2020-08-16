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
            ValidationResult.Errors.Clear();
            if (!_fileVerifierService.UnzipFiles(request.FilePath, request.FilePathExtract))
            {
                AddError("An error happend while extracting the files");
                return Task.FromResult(ValidationResult);
            }
            if (_fileVerifierService.VerifyIfFilesAreCorrupted(request.FilePathExtract))
                AddError("A corrupted file was found!");
            return Task.FromResult(ValidationResult);
        }
    }
}
