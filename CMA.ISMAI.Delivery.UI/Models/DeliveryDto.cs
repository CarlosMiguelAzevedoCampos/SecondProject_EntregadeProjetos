using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace CMA.ISMAI.Delivery.UI.Models
{
    public class DeliveryDto
    {

        [DisplayName("Nome do Estudante:")]
        public string StudentName { get; set; }
        [DisplayName("Nome da Instituição:")]
        public string InstituteName { get; set; }
        [DisplayName("Nome do Curso:")]
        public string CourseName { get; set; }
        [DisplayName("Email do Estudante:")]
        public string StudentEmail { get; set; }
        [DisplayName("Número de Aluno:")]
        public string StudentNumber { get; set; }
        [DisplayName("Nome do Coordenador:")]
        public string CordenatorName { get; set; }
        [DisplayName("Título do Projeto:")]
        public string Title { get; set; }
        [DisplayName("Definição da entrega: (Mestrado)")]
        public string DefenitionOfDelivery { get; set; } // Relatorio.., tese..
        [DisplayName("Ficheiro público de entrega (publicDelivery.pdf)")]
        public string PublicPDFVersionName { get; set; }
        [DisplayName("Ficheiro privado de entrega (privateDelivery.pdf)")]
        public string PrivatePDFVersionName { get; set; }
        [DisplayName("Entrega por Link")]
        public string FileUrl { get; set; }
        [DisplayName("Entrega por Ficheiro")]
        public IFormFile FormFile { get; set; }
        [DisplayName("Forma de entrega - Ficheiro ou Link?")]
        public bool DeliveryType { get; set; }
    }
}
