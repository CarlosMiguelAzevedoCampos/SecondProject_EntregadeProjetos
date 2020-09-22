using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Events;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Commands
{
    public class FileIdentifiersCommandHandler : CommandHandler,
         IRequestHandler<CreateFileIdentifiersCommand, ValidationResult>
    {
        private readonly IFileIdentifierService _fileIdentifierService;
        private readonly INotificationService _notificationService;
        private readonly IMediatorHandler _mediator;

        public FileIdentifiersCommandHandler(IFileIdentifierService fileIdentifierService, INotificationService notificationService, IMediatorHandler mediator)
        {
            _fileIdentifierService = fileIdentifierService;
            _notificationService = notificationService;
            _mediator = mediator;
        }

        public async Task<ValidationResult> Handle(CreateFileIdentifiersCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();
            Dictionary<string, Guid> fileInformation = _fileIdentifierService.GenerateFileIdentifier(request.FilePath);

            if (fileInformation.Count == 0)
            {
                AddError("A problem happen while generating the identifiers");
                return await Task.FromResult(ValidationResult);
            }
            else
            {
                string filesEmail = EmailTextWithIdentifiers(fileInformation);
                _notificationService.SendEmail(request.StudentEmail, filesEmail);
                _notificationService.SendEmail(request.UniversityEmail, filesEmail);
            }
                await _mediator.PublishEvent(new FilesIdentifiedEvent(request.Id, request.FilePath, request.StudentEmail));
            return await Task.FromResult(ValidationResult);

        }

        private string EmailTextWithIdentifiers(Dictionary<string, Guid> fileInforation)
        {
            StringBuilder @string = new StringBuilder(@"
                    Hello!,<br/>
                    Your delivery identifiers have been generated.
                    Here they are: <br/>");
            foreach (var item in fileInforation)
            {
                @string.Append(string.Format("<b>{0}</b> - {1} <br/> <br/>", item.Key, item.Value));
            }

            @string.Append("<br/> Thanks, <br/> Notification Service");
            return @string.ToString();
        }
    }
}
