using CMA.ISMAI.Delivery.FileProcessing.Domain.Commands;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using Moq;
using NetDevPack.Mediator;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Tests
{
    public class GenerateJuryPageCommand_Tests
    {

        [Fact(DisplayName = "Fail to get jury members")]
        [Trait("GenerateJuryPageCommand", "Generate jury page")]
        public void FailToGetJuryFromExcel()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            fileReader.Setup(x => x.ReturnJury(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<string>());
            var generateJuryPageCommand = new GenerateJuryPageCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), @"C:\DSV", It.IsAny<string>());
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object);
            var result = fileHandler.Handle(generateJuryPageCommand, new CancellationToken());

            // Assert

            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Fail to jury page")]
        [Trait("GenerateJuryPageCommand", "Generate jury page")]
        public void FailToGenerateJuryPage()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            fileReader.Setup(x => x.ReturnJury(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<string>() { "José" });

            var generateJuryPageCommand = new GenerateJuryPageCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), @"C:\DSV", It.IsAny<string>());
            generateJuryPage.Setup(x => x.AddJuryPage(It.IsAny<string>(), new List<string>())).Returns(false);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object);
            var result = fileHandler.Handle(generateJuryPageCommand, new CancellationToken());

            // Assert

            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Generate jury page went well")]
        [Trait("GenerateJuryPageCommand", "Generate jury page")]
        public void GenerateJuryPage()
        {
            //Arrange
            var pdfProcessor = new Mock<IGenerateWaterMarkService>();
            var coverProcessor = new Mock<ICoverPageService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var generateJuryPage = new Mock<IGenerateJuryPageService>();
            var generateJuryPageCommand = new GenerateJuryPageCommand(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), @"C:\DSV", It.IsAny<string>());
            var fileReader = new Mock<IFileReaderService>();
            fileReader.Setup(x => x.ReturnJury(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<string>() { "José" });
            generateJuryPage.Setup(x => x.AddJuryPage(It.IsAny<string>(), new List<string>() { "José" })).Returns(true);
            // Act
            var fileHandler = new FileProcessingHandler(pdfProcessor.Object, coverProcessor.Object, generateJuryPage.Object, fileReader.Object, meditrHandler.Object);
            var result = fileHandler.Handle(generateJuryPageCommand, new CancellationToken());

            // Assert

            Assert.True(result.Result.IsValid);
        }
    }
}
