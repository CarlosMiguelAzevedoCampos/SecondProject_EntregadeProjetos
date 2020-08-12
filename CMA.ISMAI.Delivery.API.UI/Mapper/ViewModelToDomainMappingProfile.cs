using AutoMapper;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.UI.Model;

namespace CMA.ISMAI.Delivery.API.UI.Mapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<DeliveryWithFileDto, CreateDeliveryWithFileCommand>()
                .ConstructUsing(c => new CreateDeliveryWithFileCommand(c.StudentNumber, c.CourseName, c.InstituteName,
                c.StudentName, c.StudentEmail, c.DeliveryFile, c.CordenatorName, c.Title, c.DefenitionOfDelivery, c.PublicPDFVersionName, c.PrivatePDFVersionName));
            CreateMap<DeliveryWithLinkDto, CreateDeliveryWithLinkCommand>()
                    .ConstructUsing(c => new CreateDeliveryWithLinkCommand(c.StudentNumber, c.CourseName, c.InstituteName,
                c.StudentName, c.StudentEmail, c.FileLink, c.CordenatorName, c.Title, c.DefenitionOfDelivery, c.PublicPDFVersionName, c.PrivatePDFVersionName));
        }
    }
}