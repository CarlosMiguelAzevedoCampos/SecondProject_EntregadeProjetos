using System;

namespace CMA.ISMAI.Core.Model
{
    public class Delivery
    {
        public Delivery(Guid id, string studentName, string instituteName, string courseName, string studentEmail, string studentNumber, DateTime deliveryTime, string cordenatorName, string title, string defenitionOfDelivery, string publicPDFVersionName, string privatePDFVersionName)
        {
            Id = id;
            StudentName = studentName;
            InstituteName = instituteName;
            CourseName = courseName;
            StudentEmail = studentEmail;
            StudentNumber = studentNumber;
            DeliveryTime = deliveryTime;
            CordenatorName = cordenatorName;
            Title = title;
            DefenitionOfDelivery = defenitionOfDelivery;
            PublicPDFVersionName = publicPDFVersionName;
            PrivatePDFVersionName = privatePDFVersionName;
        }

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
