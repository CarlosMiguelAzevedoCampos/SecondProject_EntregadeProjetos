using CMA.ISMAI.Core.Model;
using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model
{
    public class DownloadFileFromUrlCommand : Command
    {
        public DeliveryWithLink DeliveryWithLink { get; set; }
        public DownloadFileFromUrlCommand(Guid id, string studentName, string instituteName, string courseName, string studentEmail, string studentNumber, DateTime deliveryTime, string cordenatorName, string defenition, string title, string deliveryUrl, string publicPDFVersionName, string privatePDFVersionName)
        {
            DeliveryWithLink = new DeliveryWithLink(id, studentName, instituteName, courseName, studentEmail, studentNumber, deliveryTime, cordenatorName, defenition, title, deliveryUrl, publicPDFVersionName, privatePDFVersionName);
        }
    }
}
