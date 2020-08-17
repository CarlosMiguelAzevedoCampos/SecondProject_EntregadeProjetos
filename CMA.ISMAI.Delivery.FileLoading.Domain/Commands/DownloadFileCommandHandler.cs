using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.API.Domain.Events;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Commands
{
    public class DownloadFileCommandHandler : CommandHandler,
         IRequestHandler<DownloadFileFromUrlCommand, ValidationResult>
    {
        private readonly IHttpRequestService _httpRequestService;
        private readonly IMediatorHandler _mediator;

        public DownloadFileCommandHandler(IHttpRequestService httpRequestService, IMediatorHandler mediator)
        {
            _httpRequestService = httpRequestService;
            _mediator = mediator;
        }

        public async Task<ValidationResult> Handle(DownloadFileFromUrlCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();
            if (!_httpRequestService.DownloadFileToHost(string.Format(@"C:\Users\Carlos Campos\Desktop\Teste\Zip\{0}_{1}_{2}_{3}.zip", request.DeliveryWithLink.StudentNumber, request.DeliveryWithLink.InstituteName,
                request.DeliveryWithLink.StudentName, request.DeliveryWithLink.CourseName), request.DeliveryWithLink.Title))
            {
                AddError("An error happend while downloading!");
                return await Task.FromResult(ValidationResult);
            }

            await _mediator.PublishEvent(new CreateDeliveryWithFileEvent(new DeliveryFileSystem(request.DeliveryWithLink.Id, request.DeliveryWithLink.StudentName, request.DeliveryWithLink.InstituteName, request.DeliveryWithLink.CourseName,
                 request.DeliveryWithLink.StudentEmail, request.DeliveryWithLink.StudentNumber, request.DeliveryWithLink.DeliveryTime, request.DeliveryWithLink.CordenatorName, request.DeliveryWithLink.Title, request.DeliveryWithLink.DefenitionOfDelivery, request.DeliveryWithLink.PublicPDFVersionName, request.DeliveryWithLink.PrivatePDFVersionName, string.Format(@"C:\Users\Carlos Campos\Desktop\Teste\Zip\{0}_{1}_{2}_{3}.zip", request.DeliveryWithLink.StudentNumber, request.DeliveryWithLink.InstituteName,
            request.DeliveryWithLink.StudentName, request.DeliveryWithLink.CourseName))));
           
            return await Task.FromResult(ValidationResult);
        }
    }
}
