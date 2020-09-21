using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Commands
{
    public class VerifyFileCommandHandler : CommandHandler,
        IRequestHandler<VerifyFilesCommand, ValidationResult>
    {
        private readonly IFileVerifierService _fileVerifierService;
        private readonly IMediatorHandler _mediator;

        public VerifyFileCommandHandler(IFileVerifierService fileVerifierService, IMediatorHandler mediator)
        {
            _fileVerifierService = fileVerifierService;
            _mediator = mediator;
        }

        public async Task<ValidationResult> Handle(VerifyFilesCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            if (_fileVerifierService.VerifyIfFilesAreCorrupted(request.FilePathExtract))
            {
                AddError("A corrupted file was found or some file was found with an incorrect extesion!");
                return await Task.FromResult(ValidationResult);
            }
            await _mediator.PublishEvent(new FilesVerifiedEvent(request.Id, request.FilePathExtract));
            return await Task.FromResult(ValidationResult);
        }
    }
}
