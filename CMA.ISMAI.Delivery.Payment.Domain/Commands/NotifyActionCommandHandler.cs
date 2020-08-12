using CMA.ISMAI.Core.Model;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using FluentValidation.Results;
using MediatR;
using NetDevPack.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.Payment.Domain.Commands
{
    public class NotifyActionCommandHandler : CommandHandler,
         IRequestHandler<NotifyActionCommand, ValidationResult>
    {
        private readonly INotifyService _notifyService;

        public NotifyActionCommandHandler(INotifyService notifyService)
        {
            _notifyService = notifyService;
        }
        public Task<ValidationResult> Handle(NotifyActionCommand request, CancellationToken cancellationToken)
        {
            if (!_notifyService.SendEmail(request.Body, request.Email))
                AddError("A problem happend while sending the email");
            return Task.FromResult(ValidationResult);
        }
    }
}
