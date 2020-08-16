using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Commands
{
    public class FileProcessingHandler : CommandHandler,
        IRequestHandler<GenerateWaterMarkCommand, ValidationResult>,
        IRequestHandler<GenerateCoverPageCommand, ValidationResult>,
        IRequestHandler<GenerateJuryPageCommand, ValidationResult>
    {
        private readonly IGenerateWaterMarkService _generateWaterMarkService;
        private readonly ICoverPageService _coverPageService;
        private readonly IGenerateJuryPageService _generateJuryPageService;
        private readonly IFileReaderService _fileReaderService;

        public FileProcessingHandler(IGenerateWaterMarkService generateWaterMarkService, ICoverPageService coverPageService, IGenerateJuryPageService generateJuryPageService, IFileReaderService fileReaderService)
        {
            _generateWaterMarkService = generateWaterMarkService;
            _coverPageService = coverPageService;
            _generateJuryPageService = generateJuryPageService;
            _fileReaderService = fileReaderService;
        }

        public Task<ValidationResult> Handle(GenerateWaterMarkCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            if (!_generateWaterMarkService.AddWaterMark(request.FilePath, request.PublicPDFVersionName, request.PrivatePDFVersionName))
                AddError("A problem happen while adding a water mark");
            return Task.FromResult(ValidationResult);
        }

        public Task<ValidationResult> Handle(GenerateCoverPageCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            if (!_coverPageService.AddCoverPage(request.FilePath, request.Title, request.StudentName, request.Cordenator, request.DeliveryContext))
                AddError("A problem happen while adding a cover");
            return Task.FromResult(ValidationResult);
        }

        public Task<ValidationResult> Handle(GenerateJuryPageCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();

            List<string> jury = _fileReaderService.ReturnJury(request.StudentNumber, request.StudentInstituteName, request.StudentCourseName);
            if (jury.Count == 0)
            {
                AddError("No jury was found");
                return Task.FromResult(ValidationResult);
            }
            if (!_generateJuryPageService.AddJuryPage(request.FilePath, jury))
                AddError("A problem happen while adding the jury page");
            return Task.FromResult(ValidationResult);
        }
    }
}
