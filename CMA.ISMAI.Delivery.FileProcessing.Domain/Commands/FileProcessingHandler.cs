using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Events;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Commands
{
    public class FileProcessingHandler : CommandHandler,
        IRequestHandler<GenerateWaterMarkCommand, ValidationResult>,
        IRequestHandler<GenerateCoverPageCommand, ValidationResult>,
        IRequestHandler<GenerateJuryPageCommand, ValidationResult>,
        IRequestHandler<FileTransferCommand, ValidationResult>
    {
        private readonly IGenerateWaterMarkService _generateWaterMarkService;
        private readonly ICoverPageService _coverPageService;
        private readonly IGenerateJuryPageService _generateJuryPageService;
        private readonly IFileReaderService _fileReaderService;
        private readonly IMediatorHandler _mediator;
        private readonly IFileTransferService _fileTransferService;

        public FileProcessingHandler(IGenerateWaterMarkService generateWaterMarkService, ICoverPageService coverPageService, IGenerateJuryPageService generateJuryPageService, IFileReaderService fileReaderService, IMediatorHandler mediator, IFileTransferService fileTransferService)
        {
            _generateWaterMarkService = generateWaterMarkService;
            _coverPageService = coverPageService;
            _generateJuryPageService = generateJuryPageService;
            _fileReaderService = fileReaderService;
            _mediator = mediator;
            _fileTransferService = fileTransferService;
        }

        public async Task<ValidationResult> Handle(GenerateWaterMarkCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            if (!_generateWaterMarkService.AddWaterMark(request.FilePath, request.PublicPDFVersionName, request.PrivatePDFVersionName))
            {
                AddError("A problem happen while adding a water mark");
                return await Task.FromResult(ValidationResult);
            }
           await _mediator.PublishEvent(new WaterMarkGeneratedEvent(request.StudentNumber, request.FilePath, request.CourseName, request.InstitutionName,
                request.DeliveryDate, request.PublicPDFVersionName, request.PrivatePDFVersionName));
            return await Task.FromResult(ValidationResult);

        }

        public async Task<ValidationResult> Handle(GenerateCoverPageCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            if (!_coverPageService.AddCoverPage(request.FilePath, request.Title, request.StudentName, request.Cordenator, request.DeliveryContext, request.CoverImagePath))
            {
                AddError("A problem happen while adding a cover");
                return await Task.FromResult(ValidationResult);
            }
            await _mediator.PublishEvent(new CoverPageGeneratedEvent(request.StudentName, request.Cordenator, request.DeliveryContext, request.Title, request.FilePath));
            return await Task.FromResult(ValidationResult);

        }

        public async Task<ValidationResult> Handle(GenerateJuryPageCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            List<string> jury = _fileReaderService.ReturnJury(request.StudentNumber, request.StudentInstituteName, request.StudentCourseName, request.JuryFile);
            if (jury.Count == 0)
            {
                AddError("No jury was found");
                return await Task.FromResult(ValidationResult);
            }
            if (!_generateJuryPageService.AddJuryPage(request.FilePath, jury))
            {
                AddError("A problem happen while adding the jury page");
                return await Task.FromResult(ValidationResult);
            }
            await _mediator.PublishEvent(new JuryPageGeneretedEvent(request.StudentName, request.StudentNumber, request.StudentCourseName, request.StudentInstituteName,
                request.FilePath));
            return await Task.FromResult(ValidationResult);

        }

        public async Task<ValidationResult> Handle(FileTransferCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            bool result = _fileTransferService.TransferFile(request.FilePath, request.OneDrivePath, string.Format("{0}_{1}", request.StudentEmail, request.StudentNumber));

            if(!result)
            {
                AddError("File transfer failed!");
                return await Task.FromResult(ValidationResult);
            }
            await _mediator.PublishEvent(new FileTransferCompletedEvent(request.StudentEmail, request.FilePath, request.OneDrivePath));
            return await Task.FromResult(ValidationResult);
        }
    }
}
