using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using FluentValidation.Results;
using MediatR;
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

        public FileIdentifiersCommandHandler(IFileIdentifierService fileIdentifierService, INotificationService notificationService)
        {
            _fileIdentifierService = fileIdentifierService;
            _notificationService = notificationService;
        }

        public Task<ValidationResult> Handle(CreateFileIdentifiersCommand request, CancellationToken cancellationToken)
        {
            Dictionary<string, Guid> fileInforation = _fileIdentifierService.GenerateFileIdentifier(request.FilePath);

            if (fileInforation.Count == 0)
                AddError("A problem happen while generating the identifiers");
            else
                _notificationService.SendEmail(request.StudentEmail, EmailTextWithIdentifiers(fileInforation));
            
            return Task.FromResult(ValidationResult);
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
