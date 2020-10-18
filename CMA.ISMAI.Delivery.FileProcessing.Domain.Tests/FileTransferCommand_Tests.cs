using CMA.ISMAI.Delivery.FileProcessing.Domain.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models.Commands;
using Moq;
using NetDevPack.Mediator;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Tests
{
    public class FileTransferCommand_Tests
    {
        [Fact(DisplayName = "TransferFile went well")]
        [Trait("FileTransferCommand", "File transfer")]
        public void TransferFiles()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var fileTransfer = new Mock<IFileTransferService>();

            var fileTransferCommand = new FileTransferCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            fileTransfer.Setup(x => x.TransferFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object,fileTransfer.Object);
            var result = fileHandler.Handle(fileTransferCommand, new CancellationToken());

            // Assert

            Assert.True(result.Result.IsValid);
        }

        [Fact(DisplayName = "TransferFile failed")]
        [Trait("FileTransferCommand", "File transfer")]
        public void TransferFilesFailed()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var fileTransfer = new Mock<IFileTransferService>();

            var fileTransferCommand = new FileTransferCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            fileTransfer.Setup(x => x.TransferFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object, fileTransfer.Object);
            var result = fileHandler.Handle(fileTransferCommand, new CancellationToken());

            // Assert

            Assert.False(result.Result.IsValid);
        }

    }
}
