using CMA.ISMAI.Delivery.FileLoading.Domain.Commands;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Tests
{
    public class DownloadFileService_Tests
    {
        [Fact(DisplayName = "Everything went ok")]
        [Trait("DownloadFileFromUrlCommand", "Verify file download")]
        public void VerifyFileDownload_Success()
        {
            // Arrange
            DownloadFileFromUrlCommand donwloadCommand = new DownloadFileFromUrlCommand(Guid.NewGuid(), "", "ISMAI", "Informática", "a029216@ismai.pt", "a029216", DateTime.Now, "José", "Mestado", "Mestrado", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "Publico", "Privado");
            var httpRequest = new Mock<IHttpRequestService>();

            httpRequest.Setup(x => x.DownloadFileToHost(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var deliveryFileHandler = new DownloadFileCommandHandler(httpRequest.Object);

            // Act
            var result = deliveryFileHandler.Handle(donwloadCommand, new CancellationToken());

            // Assert
            Assert.True(result.Result.IsValid);
        }

        [Fact(DisplayName = "Download had a problem ok")]
        [Trait("DownloadFileFromUrlCommand", "Verify file download")]
        public void VerifyFileDownload_Fail()
        {
            // Arrange
            DownloadFileFromUrlCommand donwloadCommand = new DownloadFileFromUrlCommand(Guid.NewGuid(), "", "ISMAI", "Informática", "a029216@ismai.pt", "a029216", DateTime.Now, "José", "Mestado", "Mestrado", "https://1drv.ms/u/s!Aos6ApXpMWOBajj-TKD5KkSWA4A?e=RdfSVJ", "Publico", "Privado");
            var httpRequest = new Mock<IHttpRequestService>();

            httpRequest.Setup(x => x.DownloadFileToHost(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var deliveryFileHandler = new DownloadFileCommandHandler(httpRequest.Object);

            // Act
            var result = deliveryFileHandler.Handle(donwloadCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }
    }
}
