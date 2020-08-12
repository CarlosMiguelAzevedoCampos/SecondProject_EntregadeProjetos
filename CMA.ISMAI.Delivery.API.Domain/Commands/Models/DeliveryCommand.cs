using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.API.Domain.Commands.Models
{
    public abstract class DeliveryCommand : Command
    {
        public Guid Id { get; protected set; }
        public string StudentName { get; set; }
        public string InstituteName { get; set; }
        public string CourseName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentNumber { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string CordenatorName { get; set; }
        public string Title { get; set; }
        public string DefenitionOfDelivery { get; set; } // Relatorio.., tese..
        public string PublicPDFVersionName { get; set; }
        public string PrivatePDFVersionName { get; set; }
    }
}
