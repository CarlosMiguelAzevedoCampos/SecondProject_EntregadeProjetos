using System;

namespace CMA.ISMAI.Core.Model
{
    public class DeliveryWithLink : Delivery
    {
        public DeliveryWithLink(Guid id, string studentName, string instituteName, string courseName, string studentEmail, string studentNumber, DateTime deliveryTime, string fileUrl, string cordenatorName, string defenition, string title, string publicPDFVersionName, string privatePDFVersionName)
            : base(id, studentName, instituteName, courseName, studentEmail, studentNumber, deliveryTime, cordenatorName, title, defenition, publicPDFVersionName, privatePDFVersionName)
        {
            FileUrl = fileUrl;
        }
        public string FileUrl { get; set; }
    }
}
