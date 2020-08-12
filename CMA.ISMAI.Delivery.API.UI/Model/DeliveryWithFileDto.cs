using Microsoft.AspNetCore.Http;

namespace CMA.ISMAI.Delivery.API.UI.Model
{
    public class DeliveryWithFileDto : DeliveryDto
    {
        public IFormFile DeliveryFile { get; set; }
    }
}
