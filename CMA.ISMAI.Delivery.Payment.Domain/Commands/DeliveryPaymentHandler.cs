using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.Domain.Commands
{
    public class DeliveryPaymentHandler : CommandHandler,
         IRequestHandler<VerifyPaymentOfDeliveryCommand, ValidationResult>
    {
        private readonly IFileReaderService _fileReaderService;

        public DeliveryPaymentHandler(IFileReaderService fileReaderService)
        {
            _fileReaderService = fileReaderService;
        }
        public Task<ValidationResult> Handle(VerifyPaymentOfDeliveryCommand request, CancellationToken cancellationToken)
        {
            if (!_fileReaderService.PaymentHasBeenDone(request.StudentNumber, request.CourseName, request.InstituteName))
                AddError("Payment not done");
            return Task.FromResult(ValidationResult);
        }
    }
}
