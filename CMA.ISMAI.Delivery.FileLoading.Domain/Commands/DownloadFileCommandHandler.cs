using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Commands
{
    public class DownloadFileCommandHandler : CommandHandler,
         IRequestHandler<DownloadFileFromUrlCommand, ValidationResult>
    {
        private readonly IHttpRequestService _httpRequestService;

        public DownloadFileCommandHandler(IHttpRequestService httpRequestService)
        {
            _httpRequestService = httpRequestService;
        }

        public Task<ValidationResult> Handle(DownloadFileFromUrlCommand request, CancellationToken cancellationToken)
        {
            if (!_httpRequestService.DownloadFileToHost("", request.DeliveryWithLink.Title))
            {
                AddError("An error happend while downloading!");
            }
            return Task.FromResult(ValidationResult);
        }
    }
}
