using CMA.ISMAI.Delivery.FileProcessing.Domain.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using Moq;
using NetDevPack.Mediator;
using System;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Tests
{
    public class GenerateWaterMarkCommand_Tests
    {
        [Fact(DisplayName = "Fail to Generate water mark")]
        [Trait("GenerateWaterMarkCommand", "Generate water mark")]
        public void FailToGenerateWaterMark()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var fileTransfer = new Mock<IFileTransferService>();

            var generateWaterMark = new GenerateWaterMarkCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                It.IsAny<string>(), It.IsAny<string>());
            pdfProcessor.Setup(x => x.AddWaterMark(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object, fileTransfer.Object);
            var result = fileHandler.Handle(generateWaterMark, new CancellationToken());

            // Assert

            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Generate water mark")]
        [Trait("GenerateWaterMarkCommand", "Generate water mark")]
        public void GenerateWaterMark()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var fileTransfer = new Mock<IFileTransferService>();

            var generateWaterMark = new GenerateWaterMarkCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                It.IsAny<string>(), It.IsAny<string>());
            pdfProcessor.Setup(x => x.AddWaterMark(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object, fileTransfer.Object);
            var result = fileHandler.Handle(generateWaterMark, new CancellationToken());

            // Assert

            Assert.True(result.Result.IsValid);
        }
    }
}
