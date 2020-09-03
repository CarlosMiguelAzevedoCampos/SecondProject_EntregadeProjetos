using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Events;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Handlers
{
    public class CreateDeliveryWithFileCommandHandler : CommandHandler,
        IRequestHandler<CreateDeliveryWithFileCommand, ValidationResult>
    {
        private readonly IZipFileService _zipFileService;
        private readonly IMediatorHandler _mediator;
        private readonly IQueueService _queueService;
        private readonly IFileSaverService _fileSaverService;
        private readonly IConfiguration _config;

        
        public CreateDeliveryWithFileCommandHandler(IZipFileService zipFileService, IMediatorHandler mediator, IQueueService queueService, IFileSaverService fileSaverService)
        {
            _zipFileService = zipFileService;
            _mediator = mediator;
            _queueService = queueService;
            _fileSaverService = fileSaverService;
            _config = new ConfigurationBuilder()
                                                                 .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                                                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                                 .AddEnvironmentVariables()
                                                                 .Build();
        }

        public async Task<ValidationResult> Handle(CreateDeliveryWithFileCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return request.ValidationResult;
            if (!_zipFileService.DoesZipFileContainsFiles(request.DeliveryFile))
            {
                AddError("Zip file dosen't contains files!");
                return ValidationResult;
            }
            if (!_zipFileService.DoesTheZipFileContainsaPDF(request.DeliveryFile))
            {
                AddError("No PDF file was found.");
                return ValidationResult;
            }
            if (!_zipFileService.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(request.DeliveryFile, request.PublicPDFVersionName, request.PrivatePDFVersionName))
            {
                AddError("No public or private PDF version of your project was found.");
                return ValidationResult;
            }

            if (!await _fileSaverService.DownloadFile(request.DeliveryFile, string.Format(@"{0}\{1}_{2}_{3}_{4}.zip", _config.GetSection("FilePathZip:Path").Value, request.StudentNumber, request.InstituteName,
               request.StudentName, request.CourseName)))
            {
                AddError("Failed to save file!");
                return ValidationResult;
            }

            if (!_queueService.SendToQueue(new DeliveryFileSystem(request.Id, request.StudentName, request.InstituteName, request.CourseName,
                    request.StudentEmail, request.StudentNumber, request.DeliveryTime, request.CordenatorName, request.Title, request.DefenitionOfDelivery, request.PublicPDFVersionName, request.PrivatePDFVersionName, string.Format(@"{0}\{1}_{2}_{3}_{4}.zip", _config.GetSection("FilePathZip:Path").Value, request.StudentNumber, request.InstituteName,
               request.StudentName, request.CourseName)), "FileLoading"))
            {
                _fileSaverService.DeleteFile(string.Format(@"{0}\{1}_{2}_{3}_{4}.zip", _config.GetSection("FilePathZip:Path").Value, request.StudentNumber, request.InstituteName,
               request.StudentName, request.CourseName));
                AddError("A problem happend while sending your submition to the Queue.");
                return ValidationResult;
            }

            
            await _mediator.PublishEvent(new CreateDeliveryWithFileEvent(new DeliveryFileSystem(request.Id, request.StudentName, request.InstituteName, request.CourseName,
                    request.StudentEmail, request.StudentNumber, request.DeliveryTime, request.CordenatorName, request.Title, request.DefenitionOfDelivery, request.PublicPDFVersionName, request.PrivatePDFVersionName, string.Format(@"{0}\{1}_{2}_{3}_{4}.zip", _config.GetSection("FilePathZip:Path").Value, request.StudentNumber, request.InstituteName,
               request.StudentName, request.CourseName))));
            return ValidationResult;
        }
    }
}
