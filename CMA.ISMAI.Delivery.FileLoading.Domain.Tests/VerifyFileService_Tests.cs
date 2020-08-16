using CMA.ISMAI.Delivery.FileLoading.Domain.Commands;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Tests
{
    public class VerifyFileService_Tests
    {
        [Fact(DisplayName = "Failed to extract files")]
        [Trait("VerifyFilesCommand", "Verify if files are corrupted")]
        public void FailedToExtractFiles()
        {
            // Arrange
            VerifyFilesCommand verifyFilesCommand = new VerifyFilesCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>());
            var fileVerifier = new Mock<IFileVerifierService>();
            fileVerifier.Setup(x => x.UnzipFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            VerifyFileCommandHandler verifyFileCommandHandler = new VerifyFileCommandHandler(fileVerifier.Object);

            // Act
            var result = verifyFileCommandHandler.Handle(verifyFilesCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }


        [Fact(DisplayName = "Verify file went ok")]
        [Trait("VerifyFilesCommand", "Verify if files are corrupted")]
        public void FilesAreNotCorrupted()
        {
            // Arrange
            VerifyFilesCommand verifyFilesCommand = new VerifyFilesCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>());
            var fileVerifier = new Mock<IFileVerifierService>();
            fileVerifier.Setup(x => x.VerifyIfFilesAreCorrupted(It.IsAny<string>())).Returns(false);
            fileVerifier.Setup(x => x.UnzipFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            VerifyFileCommandHandler verifyFileCommandHandler = new VerifyFileCommandHandler(fileVerifier.Object);

            // Act
            var result = verifyFileCommandHandler.Handle(verifyFilesCommand, new CancellationToken());

            // Assert
            Assert.True(result.Result.IsValid);
        }

        [Fact(DisplayName = "Files are corrupted")]
        [Trait("VerifyFilesCommand", "Verify if files are corrupted")]
        public void FilesAreCorrupted()
        {
            // Arrange
            VerifyFilesCommand verifyFilesCommand = new VerifyFilesCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>());
            var fileVerifier = new Mock<IFileVerifierService>();
            fileVerifier.Setup(x => x.VerifyIfFilesAreCorrupted(It.IsAny<string>())).Returns(true); 
            fileVerifier.Setup(x => x.UnzipFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            VerifyFileCommandHandler verifyFileCommandHandler = new VerifyFileCommandHandler(fileVerifier.Object);

            // Act
            var result = verifyFileCommandHandler.Handle(verifyFilesCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }
    }
}
