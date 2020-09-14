using AutoMapper;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.UI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.Mediator;
using System;
using System.Threading.Tasks;

namespace CMA.ISMAI.Delivery.API.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : BaseController
    {
        private readonly IMediatorHandler _mediator;
        private readonly IMapper _mapper;

        public DeliveriesController(IMediatorHandler mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("UploadWithFile")]
        [RequestSizeLimit(53428800)]
        public async Task<IActionResult> UploadWithFile([FromForm]DeliveryWithFileDto deliveryDto)
        {
            if (!IsAZipFile(deliveryDto.DeliveryFile))
            {
                AddError("Your file must be a ZIP file!");
                return CustomResponse();
            }
            deliveryDto.DeliveryTime = DateTime.Now;
            var registerCommand = _mapper.Map<CreateDeliveryWithFileCommand>(deliveryDto);
            var fileValidation = await _mediator.SendCommand(registerCommand);
            return CustomResponse(fileValidation, fileValidation.IsValid ? string.Format("The submition ended at {0}. The file delivered was {1}. " +
                "After this, if everything is correct, you will recive an e-mail saying that you can pay your delivery! Process ID - {2}", DateTime.Now, deliveryDto.DeliveryFile.FileName, registerCommand.Id) : string.Empty);
        }


        [HttpPost("UploadWithLink")]
        public async Task<IActionResult> UploadWithLink([FromForm]DeliveryWithLinkDto deliveryDto)
        {
            deliveryDto.DeliveryTime = DateTime.Now;
            var registerCommand = _mapper.Map<CreateDeliveryWithLinkCommand>(deliveryDto);
            var fileValidation = await _mediator.SendCommand(registerCommand);
            return CustomResponse(fileValidation, fileValidation.IsValid ? string.Format("The submition ended at {0}. The link delivered was {1}. " +
                           "After this, if everything is correct, you will recive an e-mail saying that you can pay your delivery! Process ID - {2}", DateTime.Now, deliveryDto.FileLink, registerCommand.Id) : string.Empty);
        }

        private bool IsAZipFile(IFormFile deliveryFile)
        {
            if (deliveryFile != null)
                return deliveryFile.FileName.ToLower().Contains(".zip");
            return false;
        }
    }
}