using CMA.ISMAI.Delivery.Payment.Domain.Model;
using NetDevPack.Mediator;
using System;

namespace CMA.ISMAI.Delivery.Payment.UI
{
    internal class ConsoleApplication
    {
        private readonly IMediatorHandler _mediatr;

        public ConsoleApplication(IMediatorHandler mediatr)
        {
            _mediatr = mediatr;
        }

        public void StartService()
        {
            this._mediatr.SendCommand<VerifyPaymentOfDeliveryCommand>(new VerifyPaymentOfDeliveryCommand("A029216", "ISMAI", "Informática"));
        }
    }
}