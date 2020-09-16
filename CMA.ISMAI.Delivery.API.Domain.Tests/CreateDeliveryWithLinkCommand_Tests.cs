using CMA.ISMAI.Delivery.API.Domain.Commands.Handlers;
using CMA.ISMAI.Delivery.API.Domain.Commands.Models;
using CMA.ISMAI.Delivery.API.Domain.Interfaces;
using FluentValidation.Results;
using Moq;
using NetDevPack.Mediator;
using NetDevPack.Messaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.API.Domain.Tests
{
    public class CreateDeliveryWithLinkCommand_Tests
    {
        [Fact(DisplayName = "Invalid Delivery")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void CreateaInvalidDelivery()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("", "", "", "", "", "", "", "", "", "", "");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Never);
            httpRequest.Verify(x => x.ReturnObjectFromTheUrl(It.IsAny<string>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Link is not acessible")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void LinkOfTheFileIsNotAcessible()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns((HttpWebResponse)null);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            httpRequest.Verify(x => x.IsAZipFile(It.IsAny<HttpWebResponse>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }



        [Fact(DisplayName = "Invalid file store provider - Must be OneDrive")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void DelivereTheProjectInAAlternativeHost()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            httpRequest.Verify(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Couldn't return file information")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void CouldntReciveFileInformation()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns((HttpWebResponse)null);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Couldn't return zip file")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void CouldntReturnZipFile()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }



        [Fact(DisplayName = "Fail to send to the Queue")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void FailToSendToTheQueue()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "File is bigger than 5 GB")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void FileIsBiggerThan5GB()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            // Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.IsTheFileSmallerThanFiveGB(It.IsAny<HttpWebResponse>())).Returns(false);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            // Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Everything went well on the Devilery")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void SucessOnDelivery()
        {
            // Act
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            // Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            // Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Once);
            Assert.True(result.Result.IsValid);
        }
    }
}
