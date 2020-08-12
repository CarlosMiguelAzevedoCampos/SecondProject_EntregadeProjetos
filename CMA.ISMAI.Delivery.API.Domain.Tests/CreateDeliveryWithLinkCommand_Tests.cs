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
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("", "", "", "", "", "", "", "", "", "", "");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
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
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns((HttpWebResponse)null);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
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
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
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
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns((HttpWebResponse)null);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            httpRequest.Verify(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Couldn't return zip file")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void CouldntReturnZipFile()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns((MemoryStream)null);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            zipLinkMock.Verify(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Invalid file format")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void FileIsNotAZipFile()
        {
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns(new MemoryStream());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            zipLinkMock.Verify(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Zip file dosen't contains files")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void ZipDosentContainFiles()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns(new MemoryStream());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            zipLinkMock.Verify(x => x.DoesTheZipFileContainsaPDF(It.IsAny<MemoryStream>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "File dosen't contains a PDF")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void ZipFileDosentContainsaPDF()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns(new MemoryStream());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<MemoryStream>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            zipLinkMock.Verify(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<MemoryStream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "File dosen't contains a PDF for public and private delivery")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void ZipFileDosentContainsaPDFForPublicOrPrivateDelivery()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();
            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns(new MemoryStream());
            zipLinkMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<MemoryStream>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Fail to send to the Queue")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void FailToSendToTheQueue()
        {
            // Act
            var zipMock = new Mock<IZipFileService>();
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            //Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns(new MemoryStream());
            zipLinkMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<MemoryStream>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(false);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Never);
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Everything went well on the Devilery")]
        [Trait("CreateDeliveryWithLinkCommand", "Creating the Delivery object and actions")]
        public void SucessOnDelivery()
        {
            // Act
            var zipMock = new Mock<IZipFileService>();
            var zipFileMock = new Mock<IZipFileService>();
            var zipLinkMock = new Mock<IZipUrlService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var httpRequest = new Mock<IHttpRequestService>();
            var queueMock = new Mock<IQueueService>();

            // Arrange
            httpRequest.Setup(x => x.ReturnObjectFromTheUrl(It.IsAny<string>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.IsAZipFile(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.VerifyIfLinkIsFromTheTrustedHoster(It.IsAny<HttpWebResponse>())).Returns(true);
            httpRequest.Setup(x => x.ReturnFileInformation(It.IsAny<HttpWebResponse>())).Returns(new HttpWebResponse());
            httpRequest.Setup(x => x.ReturnZipFileFromTheUrl(It.IsAny<HttpWebResponse>())).Returns(new MemoryStream());
            zipLinkMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<MemoryStream>())).Returns(true);
            zipLinkMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<MemoryStream>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithLinkCommand createDeliveryCommand = new CreateDeliveryWithLinkCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "José", "A029216 TESTE", "Mestrado", "Private", "Public");
            CreateDeliveryWithLinkCommandHandler deliveryCommandHandler = new CreateDeliveryWithLinkCommandHandler(zipLinkMock.Object, mediatrMock.Object, httpRequest.Object, queueMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            // Assert
            mediatrMock.Verify(x => x.PublishEvent(It.IsAny<Event>()), Times.Once);
            Assert.True(result.Result.IsValid);
        }
    }
}
