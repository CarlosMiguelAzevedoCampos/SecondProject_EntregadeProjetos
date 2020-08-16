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
            ValidationResult.Errors.Clear();
            if (!_httpRequestService.DownloadFileToHost(string.Format(@"C:\Users\Carlos Campos\Desktop\Teste\Zip\{0}_{1}_{2}_{3}.zip", request.DeliveryWithLink.StudentNumber, request.DeliveryWithLink.InstituteName,
                request.DeliveryWithLink.StudentName, request.DeliveryWithLink.CourseName), request.DeliveryWithLink.Title))
            {
                AddError("An error happend while downloading!");
            }
            return Task.FromResult(ValidationResult);
        }
    }
}
