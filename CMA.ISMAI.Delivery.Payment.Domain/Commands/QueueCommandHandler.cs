using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.Domain.Commands
{
    public class QueueCommandHandler : CommandHandler,
         IRequestHandler<SendToQueueCommand, ValidationResult>
    {
        private readonly IQueueService _queueService;

        public QueueCommandHandler(IQueueService queueService)
        {
            _queueService = queueService;
        }

        public Task<ValidationResult> Handle(SendToQueueCommand request, CancellationToken cancellationToken)
        {
            if(!_queueService.SendToQueue(new Core.Model.DeliveryFileSystem(request.Id, request.StudentName, request.InstituteName, request.CourseName,
                request.StudentEmail, request.StudentNumber, request.DeliveryTime, request.CordenatorName, request.Title, request.DefenitionOfDelivery, request.PublicPDFVersionName,
                request.PrivatePDFVersionName, request.DeliveryPath)))
                AddError("Failed to add to the Queue system");

            return Task.FromResult(ValidationResult);
        }
    }
}
