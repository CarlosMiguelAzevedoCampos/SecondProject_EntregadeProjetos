using System;

namespace CMA.ISMAI.Core.Model
{
    public class DeliveryFileSystem : Delivery
    {
        public DeliveryFileSystem(Guid id, string studentName, string instituteName, string courseName, string studentEmail, string studentNumber, DateTime deliveryTime, string cordenatorName, string title, string defenitionOfDelivery, string publicPDFVersionName, string privatePDFVersionName, string deliveryPath) : base(id, studentName, instituteName, courseName, studentEmail, studentNumber, deliveryTime, cordenatorName, title, defenitionOfDelivery, publicPDFVersionName, privatePDFVersionName)
        {
            DeliveryPath = deliveryPath;
        }


        public string DeliveryPath { get; set; }
    }
}
