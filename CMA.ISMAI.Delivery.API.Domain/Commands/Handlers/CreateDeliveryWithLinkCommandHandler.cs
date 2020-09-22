using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Events;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Handlers
{
    public class CreateDeliveryWithLinkCommandHandler : CommandHandler,
       IRequestHandler<CreateDeliveryWithLinkCommand, ValidationResult>
    {
        private readonly IMediatorHandler _mediator;
        private readonly IHttpRequestService _httpRequest;
        private readonly IQueueService _queueService;


        public CreateDeliveryWithLinkCommandHandler(IMediatorHandler mediator, IHttpRequestService httpRequest, IQueueService queueService)
        {
            _mediator = mediator;
            _httpRequest = httpRequest;
            _queueService = queueService;
        }

        public async Task<ValidationResult> Handle(CreateDeliveryWithLinkCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return request.ValidationResult;

            var webReponse = _httpRequest.ReturnObjectFromTheUrl(request.LinkFile);

            if (webReponse == null)
            {
                AddError("Url is not accessible.");
                return ValidationResult;
            }

            if (!_httpRequest.VerifyIfLinkIsFromTheTrustedHoster(webReponse))
            {
                AddError("Url is not accessible or the Host provider is not OneDrive");
                return ValidationResult;
            }

            webReponse = _httpRequest.ReturnFileInformation(webReponse);
            if (webReponse == null)
            {
                AddError("No file content returned.. Try again, please.");
                return ValidationResult;
            }

            if (!_httpRequest.IsAZipFile(webReponse))
            {
                AddError("Delivery files isn't a zip file.");
                return ValidationResult;
            }

            if(!_httpRequest.IsTheFileSmallerThanFiveGB(webReponse))
            {
                AddError("Your delivery is bigger than 5GB!, please, contact the University.");
                return ValidationResult;
            }

            request = VerifyFileNames(request);

            if (!_queueService.SendToQueue(new DeliveryWithLink(request.Id, request.StudentName, request.InstituteName, request.CourseName,
               request.StudentEmail, request.StudentNumber, request.DeliveryTime, request.LinkFile, request.CordenatorName, request.DefenitionOfDelivery, request.Title, request.PublicPDFVersionName, request.PrivatePDFVersionName), "FileLoading"))
            {
                AddError("A problem happend while sending your submition to the Queue.");
                return ValidationResult;
            }

            await _mediator.PublishEvent(new CreateDeliveryWithLinkEvent(new DeliveryWithLink(request.Id, request.StudentName, request.InstituteName, request.CourseName,
                    request.StudentEmail, request.StudentNumber, request.DeliveryTime, request.LinkFile, request.CordenatorName, request.DefenitionOfDelivery, request.Title, request.PublicPDFVersionName, request.PrivatePDFVersionName)));

            return ValidationResult;
        }

        private CreateDeliveryWithLinkCommand VerifyFileNames(CreateDeliveryWithLinkCommand request)
        {
            if (!request.PrivatePDFVersionName.Contains(".pdf"))
                request.PrivatePDFVersionName = string.Format("{0}.pdf", request.PrivatePDFVersionName);
            if (!request.PublicPDFVersionName.Contains(".pdf"))
                request.PublicPDFVersionName = string.Format("{0}.pdf", request.PublicPDFVersionName);
            return request;
        }
    }
}
