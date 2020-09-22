using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model.Commands;
using CMA.ISMAI.Delivery.Logging.Interface;
using Moq;
using NetDevPack.Mediator;
using System;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Tests
{
    public class VerifyFilesNameCommandHandler_Tests
    {
        [Fact(DisplayName = "Faild to unzip files")]
        [Trait("VerifyFilesNameCommand", "Verify if the zip file contains a public and a private delivery")]
        public void FailedToUnzip()
        {
            // Arrange
            VerifyFilesNameCommand verifyFilesCommand = new VerifyFilesNameCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var fileVerifier = new Mock<IFileVerifierService>();
            var loggingService = new Mock<ILoggingService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            fileVerifier.Setup(x => x.UnzipFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            VerifyFileNameCommandHandler verifyFileCommandHandler = new VerifyFileNameCommandHandler(fileVerifier.Object, meditrHandler.Object, loggingService.Object);

            // Act
            var result = verifyFileCommandHandler.Handle(verifyFilesCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Faild to find file public or private file")]
        [Trait("VerifyFilesNameCommand", "Verify if the zip file contains a public and a private delivery")]
        public void NoPrivateOrPublicFileFound()
        {
            // Arrange
            VerifyFilesNameCommand verifyFilesCommand = new VerifyFilesNameCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var fileVerifier = new Mock<IFileVerifierService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var loggingService = new Mock<ILoggingService>();

            fileVerifier.Setup(x => x.UnzipFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            fileVerifier.Setup(x => x.VerifyIfPublicAndPrivateFilesExist(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            VerifyFileNameCommandHandler verifyFileCommandHandler = new VerifyFileNameCommandHandler(fileVerifier.Object, meditrHandler.Object, loggingService.Object);

            // Act
            var result = verifyFileCommandHandler.Handle(verifyFilesCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Founded file public or private file")]
        [Trait("VerifyFilesNameCommand", "Verify if the zip file contains a public and a private delivery")]
        public void PrivateOrPublicFileFound()
        {
            // Arrange
            VerifyFilesNameCommand verifyFilesCommand = new VerifyFilesNameCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var fileVerifier = new Mock<IFileVerifierService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var loggingService = new Mock<ILoggingService>();

            fileVerifier.Setup(x => x.UnzipFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            fileVerifier.Setup(x => x.VerifyIfPublicAndPrivateFilesExist(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            VerifyFileNameCommandHandler verifyFileCommandHandler = new VerifyFileNameCommandHandler(fileVerifier.Object, meditrHandler.Object, loggingService.Object);

            // Act
            var result = verifyFileCommandHandler.Handle(verifyFilesCommand, new CancellationToken());

            // Assert
            Assert.True(result.Result.IsValid);
        }
    }
}
