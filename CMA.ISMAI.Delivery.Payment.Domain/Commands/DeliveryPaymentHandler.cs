using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using CMA.ISMAI.Delivery.Payment.Domain.Model.Events;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.Domain.Commands
{
    public class DeliveryPaymentHandler : CommandHandler,
         IRequestHandler<VerifyPaymentOfDeliveryCommand, ValidationResult>
    {
        private readonly IFileReaderService _fileReaderService;
        private readonly IMediatorHandler _mediator;

        public DeliveryPaymentHandler(IFileReaderService fileReaderService, IMediatorHandler mediator)
        {
            _fileReaderService = fileReaderService;
            _mediator = mediator;
        }

        public async Task<ValidationResult> Handle(VerifyPaymentOfDeliveryCommand request, CancellationToken cancellationToken)
        {
            ValidationResult.Errors.Clear();
            if (!_fileReaderService.PaymentHasBeenDone(request.StudentNumber, request.CourseName, request.InstituteName, request.FilePath))
            {
                AddError("Payment not done");
                return await Task.FromResult(ValidationResult);
            }
            await _mediator.PublishEvent(new PaymentCompletedEvent(request.StudentNumber, request.InstituteName, request.CourseName));
            return await Task.FromResult(ValidationResult);
        }
    }
}
