using CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using Moq;
using Xunit;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.UnitTesting
{
    public class FileVerifierService_Tests
    {
        [Fact(DisplayName = "PDF Files Are Corrupted")]
        [Trait("FileVerifierService", "Verify if a file is corrupted")]
        public void PDFFilesAreCorrupted()
        {
            // Arrange
            var pdfService = new Mock<IPDFVerifierService>();
            var mediaService = new Mock<IVerifyFilesExtensions>();
            var logService = new Mock<ILoggingService>();
            pdfService.Setup(x => x.ArePdfFilesOk(It.IsAny<string>())).Returns(false);
            FileVerifierService fileVerifierService = new FileVerifierService(pdfService.Object, mediaService.Object, logService.Object);
            // Act
            bool result = fileVerifierService.VerifyIfFilesAreCorrupted(It.IsAny<string>());

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "Media Files Are Corrupted")]
        [Trait("FileVerifierService", "Verify if a file is corrupted")]
        public void MediaFileAreCorrupted()
        {
            // Arrange
            var pdfService = new Mock<IPDFVerifierService>();
            var mediaService = new Mock<IVerifyFilesExtensions>();
            var logService = new Mock<ILoggingService>();
            mediaService.Setup(x => x.AreMediaFilesOk(It.IsAny<string>())).Returns(false);
            FileVerifierService fileVerifierService = new FileVerifierService(pdfService.Object, mediaService.Object, logService.Object);
            // Act
            bool result = fileVerifierService.VerifyIfFilesAreCorrupted(It.IsAny<string>());

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "No Files Are Corrupted")]
        [Trait("FileVerifierService", "Verify if a file is corrupted")]
        public void NoFileIsCorrupted()
        {
            // Arrange
            var pdfService = new Mock<IPDFVerifierService>();
            var mediaService = new Mock<IVerifyFilesExtensions>();
            var logService = new Mock<ILoggingService>();

            mediaService.Setup(x => x.AreMediaFilesOk(It.IsAny<string>())).Returns(true);
            pdfService.Setup(x => x.ArePdfFilesOk(It.IsAny<string>())).Returns(true);
            FileVerifierService fileVerifierService = new FileVerifierService(pdfService.Object, mediaService.Object, logService.Object);
            // Act
            bool result = fileVerifierService.VerifyIfFilesAreCorrupted(It.IsAny<string>());

            // Assert
            Assert.False(result);
        }
    }
}
