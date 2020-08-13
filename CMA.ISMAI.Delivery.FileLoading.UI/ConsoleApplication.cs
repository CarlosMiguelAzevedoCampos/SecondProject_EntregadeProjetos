using CMA.ISMAI.Delivery.FileLoading.CrossCutting.Camunda.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using NetDevPack.Mediator;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.UI
{
    internal class ConsoleApplication
    {
        private readonly IMediatorHandler _mediatr;
        private readonly ICamundaService _ca;

        public ConsoleApplication(IMediatorHandler mediatr, ICamundaService ca)
        {
            _mediatr = mediatr;
            _ca = ca;
        }

        public void StartService()
        {
            _ca.StartWorkFlow(new Core.Model.DeliveryWithLink(Guid.NewGuid(), "Carlos", "ISMAI", "Informática", "a029216@ismai.pt", "a029216", DateTime.Now, "", "José", "Mestrado", "Jose", "safa", "safas"));
        }
    }
}