using CMA.ISMAI.Delivery.API.Domain.Commands.Handlers;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NetDevPack.Mediator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.API.Domain.Tests
{
    public class CreateDeliveryWithFileCommand_Tests
    {
        [Fact(DisplayName = "Invalid Delivery")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void CreateaInvalidDelivery()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("", "", "", "", "", It.IsAny<IFormFile>(), "", "", "", "", "");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

     

        [Fact(DisplayName = "Fail to send to the Queue")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void FailToSendToTheQueue()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();
            //Arrange
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }
         
        [Fact(DisplayName = "Fail to save file to disk")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void FailToSaveFileToDisk()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            fileMock.Setup(x => x.DownloadFile(It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Saved file to disk")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void SaveFileToDisk()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            fileMock.Setup(x => x.DownloadFile(It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);

            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.True(result.Result.IsValid);
        }


        [Fact(DisplayName = "Everything went well on the Devilery")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void SucessOnDelivery()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            fileMock.Setup(x => x.DownloadFile(It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.True(result.Result.IsValid);
        }
    }
}
