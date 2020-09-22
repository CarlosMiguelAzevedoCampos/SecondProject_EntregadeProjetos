using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Commands;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events;
using CMA.ISMAI.Delivery.Logging.Interface;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class VerifyFileNameCommandHandler : CommandHandler,
      IRequestHandler<VerifyFilesNameCommand, ValidationResult>
{
    private readonly IFileVerifierService _fileVerifierService;
    private readonly IMediatorHandler _mediator;
    private readonly ILoggingService _loggingService;

    public VerifyFileNameCommandHandler(IFileVerifierService fileVerifierService, IMediatorHandler mediator, ILoggingService loggingService)
    {
        _fileVerifierService = fileVerifierService;
        _mediator = mediator;
        _loggingService = loggingService;
    }

    public async Task<ValidationResult> Handle(VerifyFilesNameCommand request, CancellationToken cancellationToken)
    {
        ValidationResult.Errors.Clear();
        if (!_fileVerifierService.UnzipFiles(request.FilePath, request.FilePathExtract))
        {
            AddError("An error happend while extracting the files");
            return await Task.FromResult(ValidationResult);
        }

        if (!_fileVerifierService.VerifyIfPublicAndPrivateFilesExist(request.FilePathExtract, request.PrivateFile, request.PublicFile))
        {
            AddError("Public or Private file not found!");
            return await Task.FromResult(ValidationResult);
        }
         await _mediator.PublishEvent(new FilesNameVerifiedEvent(request.Id, request.FilePath, request.FilePathExtract, request.PrivateFile, request.PublicFile));
        return await Task.FromResult(ValidationResult);
    }
}
