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
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("", "", "", "", "", It.IsAny<IFormFile>(), "", "", "", "", "");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Zip file dosen't have anything")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void NoFilesFoundedInsideTheZipFile()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            List<string> files = new List<string>();
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(false);
            var file = It.IsAny<IFormFile>();

            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Fail to send to the Queue")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void FailToSendToTheQueue()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();
            //Arrange
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(false);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "No PDF file was found")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void NoPDFFileWasFound()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<IFormFile>())).Returns(false);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "No public and private pdf delivery was found")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void NoPublicAndPrivateDeliveryWasFound()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<IFormFile>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Fail to save file to disk")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void FailToSaveFileToDisk()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            fileMock.Setup(x => x.DownloadFile(It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<IFormFile>())).Returns(true);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Saved file to disk")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void SaveFileToDisk()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            fileMock.Setup(x => x.DownloadFile(It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<IFormFile>())).Returns(true);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.True(result.Result.IsValid);
        }


        [Fact(DisplayName = "Everything went well on the Devilery")]
        [Trait("CreateDeliveryCommand", "Creating the Delivery object and actions")]
        public void SucessOnDelivery()
        {
            // Act
            var zipFileMock = new Mock<IZipFileService>();
            var mediatrMock = new Mock<IMediatorHandler>();
            var queueMock = new Mock<IQueueService>();
            var fileMock = new Mock<IFileSaverService>();

            //Arrange
            fileMock.Setup(x => x.DownloadFile(It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            zipFileMock.Setup(x => x.DoesZipFileContainsFiles(It.IsAny<IFormFile>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsaPDF(It.IsAny<IFormFile>())).Returns(true);
            queueMock.Setup(x => x.SendToQueue(It.IsAny<Core.Model.Delivery>(), It.IsAny<string>())).Returns(true);
            zipFileMock.Setup(x => x.DoesTheZipFileContainsAVersionForPublicAndPrivateDelivery(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            CreateDeliveryWithFileCommand createDeliveryCommand = new CreateDeliveryWithFileCommand("A029216", "Informática", "ISMAI", "Carlos Campos", "A029216@ismai.pt", It.IsAny<IFormFile>(), "José", "Carlos", "Mestrado", "A029216_Public", "A029216_Private");
            CreateDeliveryWithFileCommandHandler deliveryCommandHandler = new CreateDeliveryWithFileCommandHandler(zipFileMock.Object, mediatrMock.Object, queueMock.Object, fileMock.Object);
            Task<ValidationResult> result = deliveryCommandHandler.Handle(createDeliveryCommand, new CancellationToken());
            //Assert
            Assert.True(result.Result.IsValid);
        }
    }
}
