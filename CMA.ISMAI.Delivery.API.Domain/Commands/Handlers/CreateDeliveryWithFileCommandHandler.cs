using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Events;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Configuration;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Handlers
{
    public class CreateDeliveryWithFileCommandHandler : CommandHandler,
        IRequestHandler<CreateDeliveryWithFileCommand, ValidationResult>
    {
        private readonly IMediatorHandler _mediator;
        private readonly IQueueService _queueService;
        private readonly IFileSaverService _fileSaverService;
        private readonly IConfiguration _config;


        public CreateDeliveryWithFileCommandHandler(IMediatorHandler mediator, IQueueService queueService, IFileSaverService fileSaverService)
        {
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


            if (!await _fileSaverService.DownloadFile(request.DeliveryFile, string.Format(@"{0}\{1}_{2}_{3}_{4}.zip", _config.GetSection("FilePathZip:Path").Value, request.StudentNumber, request.InstituteName,
               request.StudentName, request.CourseName)))
            {
                AddError("Failed to save file!");
                return ValidationResult;
            }
            request = VerifyFileNames(request);
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

        private CreateDeliveryWithFileCommand VerifyFileNames(CreateDeliveryWithFileCommand request)
        {
            if (!request.PrivatePDFVersionName.Contains(".pdf"))
                request.PrivatePDFVersionName = string.Format("{0}.pdf", request.PrivatePDFVersionName);
            if (!request.PublicPDFVersionName.Contains(".pdf"))
                request.PublicPDFVersionName = string.Format("{0}.pdf", request.PublicPDFVersionName);
            return request;
        }
    }
}
