using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;

namespace CMA.ISMAI.Delivery.UI.Models
{
    public class DeliveryDto
    {

        [DisplayName("Student Name:")]
        public string StudentName { get; set; }
        [DisplayName("Institute Name:")]
        public string InstituteName { get; set; }
        [DisplayName("Course Name:")]
        public string CourseName { get; set; }
        [DisplayName("Student Email:")]
        public string StudentEmail { get; set; }
        [DisplayName("Student Number:")]
        public string StudentNumber { get; set; }
        [DisplayName("Coordenator Name:")]
        public string CordenatorName { get; set; }
        [DisplayName("Project Title:")]
        public string Title { get; set; }
        [DisplayName("Project Defenition: (Master Degree)")]
        public string DefenitionOfDelivery { get; set; } // Relatorio.., tese..
        [DisplayName("File for public delivery (publicDelivery.pdf)")]
        public string PublicPDFVersionName { get; set; }
        [DisplayName("File for private delivery (privateDelivery.pdf)")]
        public string PrivatePDFVersionName { get; set; }
        [DisplayName("Delivery by Link")]
        public string FileUrl { get; set; }
        [DisplayName("Delivery by File")]
        public IFormFile FormFile { get; set; }
        [DisplayName("Delivery Type - File or Link?")]
        public bool DeliveryType { get; set; }
    }
}
