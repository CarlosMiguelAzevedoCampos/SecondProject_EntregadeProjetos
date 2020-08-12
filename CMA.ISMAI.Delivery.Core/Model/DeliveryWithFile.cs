using System;

namespace CMA.ISMAI.Core.Model
{
    public class DeliveryWithFile : Delivery
    {
        public DeliveryWithFile(Guid id, string studentName, string instituteName, string courseName, string studentEmail, string studentNumber, DateTime deliveryTime, string cordenatorName, string defenition, string title, string deliveryPath, string publicPDFVersionName, string privatePDFVersionName)
                    : base(id, studentName, instituteName, courseName, studentEmail, studentNumber, deliveryTime, cordenatorName, title, defenition, publicPDFVersionName, privatePDFVersionName)

        {
            DeliveryPath = deliveryPath;
        }
        public string DeliveryPath { get; set; }
    }
}
