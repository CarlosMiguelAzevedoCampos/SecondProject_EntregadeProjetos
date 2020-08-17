using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Models
{
    public class GenerateWaterMarkCommand : Command
    {
        public GenerateWaterMarkCommand(string studentNumber, string filePath, string courseName, string institutionName, DateTime deliveryDate, string publicPDFVersionName, string privatePDFVersionName)
        {
            StudentNumber = studentNumber;
            FilePath = filePath;
            CourseName = courseName;
            InstitutionName = institutionName;
            DeliveryDate = deliveryDate;
            PublicPDFVersionName = publicPDFVersionName;
            PrivatePDFVersionName = privatePDFVersionName;
        }

        public string StudentNumber { get; set; }
        public string FilePath { get; set; }
        public string CourseName { get; set; }
        public string InstitutionName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string PublicPDFVersionName { get; set; }
        public string PrivatePDFVersionName { get; set; }
    }
}
