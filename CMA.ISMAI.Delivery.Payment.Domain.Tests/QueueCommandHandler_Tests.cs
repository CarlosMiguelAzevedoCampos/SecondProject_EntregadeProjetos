using CMA.ISMAI.Delivery.Payment.Domain.Commands;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using Moq;
using System;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.Payment.Domain.Tests
{
    public class QueueCommandHandler_Tests
    {
        [Fact(DisplayName = "Fail to Send to queue")]
        [Trait("QueueCommandHandler", "Send to Queue")]
        public void VerifyIfThePaymentInTheFile_Sucess()
        {
            // Arrange
            var queueCommand = new SendToQueueCommand(Guid.NewGuid(), "Carlos Campos", "ISMAI", "Informática", "a029216@ismai.pt", "A029216", DateTime.Now, "José",
                "Title", "Mestrado", "A029216_public", "A029216_private", "Z:\\ISMAI");
            var queueService = new Mock<IQueueService>();

            queueService.Setup(x => x.SendToQueue(It.IsAny<Core.Model.DeliveryFileSystem>())).Returns(false);
            var queueHandler = new QueueCommandHandler(queueService.Object);

            // Act
            var result = queueHandler.Handle(queueCommand, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Send to queue")]
        [Trait("QueueCommandHandler", "Send to Queue")]
        public void VerifyIfThePaymentInTheFile_Fail()
        {
            // Arrange
            var queueCommand = new SendToQueueCommand(Guid.NewGuid(), "Carlos Campos", "ISMAI", "Informática", "a029216@ismai.pt", "A029216", DateTime.Now, "José", 
                "Title", "Mestrado", "A029216_public", "A029216_private", "Z:\\ISMAI");
            var queueService = new Mock<IQueueService>();

            queueService.Setup(x => x.SendToQueue(It.IsAny<Core.Model.DeliveryFileSystem>())).Returns(true);
            var queueHandler = new QueueCommandHandler(queueService.Object);

            // Act
            var result = queueHandler.Handle(queueCommand, new CancellationToken());
            Assert.True(result.Result.IsValid);
        }
    }
}
