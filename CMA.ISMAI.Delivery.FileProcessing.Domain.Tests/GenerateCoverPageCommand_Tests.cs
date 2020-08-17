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
    public class GenerateCoverPageCommand_Tests
    {
        [Fact(DisplayName = "Fail to add cover")]
        [Trait("GenerateCoverPageCommand", "Generate cover")]
        public void FailToGenerateCover()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();

            var generateCoverPageCommand = new GenerateCoverPageCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            coverProcessor.Setup(x => x.AddCoverPage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object);
            var result = fileHandler.Handle(generateCoverPageCommand, new CancellationToken());

            // Assert

            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Generate cover for the PDF File")]
        [Trait("GenerateCoverPageCommand", "Generate cover")]
        public void GenerateCover()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();

            var generateCoverPageCommand = new GenerateCoverPageCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            coverProcessor.Setup(x => x.AddCoverPage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object);
            var result = fileHandler.Handle(generateCoverPageCommand, new CancellationToken());

            // Assert

            Assert.True(result.Result.IsValid);
        }
    }
}
