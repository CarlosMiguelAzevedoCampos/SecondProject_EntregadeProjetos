using CMA.ISMAI.Core.Interface;
using CMA.ISMAI.Delivery.FileLoading.Domain.Commands;
using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using Moq;
using NetDevPack.Mediator;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Tests
{
    public class FileIdentifiersService_Tests
    {
        [Fact(DisplayName = "Identifiers Generated")]
        [Trait("FileIdentifiersCommandHandler", "Verify file download")]
        public void VerifyFileDownload_Success()
        {
            // Arrange
            CreateFileIdentifiersCommand createFileIdentifiersCommand = new CreateFileIdentifiersCommand(Guid.NewGuid(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>());
            var fileIdentifierService = new Mock<IFileIdentifierService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            var notifyService = new Mock<INotificationService>();
            Dictionary<string, Guid> fileInfo = new Dictionary<string, Guid>();
            fileInfo.Add("public", Guid.NewGuid());
            fileIdentifierService.Setup(x => x.GenerateFileIdentifier(It.IsAny<string>())).Returns(fileInfo);
            var deliveryFileHandler = new FileIdentifiersCommandHandler(fileIdentifierService.Object, notifyService.Object, meditrHandler.Object);

            // Act
            var result = deliveryFileHandler.Handle(createFileIdentifiersCommand, new CancellationToken());

            // Assert
            Assert.True(result.Result.IsValid);
        }

        [Fact(DisplayName = "Identifiers failed Generated")]
        [Trait("FileIdentifiersCommandHandler", "Verify file download")]
        public void VerifyFileDownload_Fail()
        {
            // Arrange
            CreateFileIdentifiersCommand createFileIdentifiersCommand = new CreateFileIdentifiersCommand(Guid.NewGuid(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var fileIdentifierService = new Mock<IFileIdentifierService>();
            var notifyService = new Mock<INotificationService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            Dictionary<string, Guid> fileInfo = new Dictionary<string, Guid>();
            fileIdentifierService.Setup(x => x.GenerateFileIdentifier(It.IsAny<string>())).Returns(fileInfo);
            var deliveryFileHandler = new FileIdentifiersCommandHandler(fileIdentifierService.Object, notifyService.Object, meditrHandler.Object);

            // Act
            var result = deliveryFileHandler.Handle(createFileIdentifiersCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }
    }
}
