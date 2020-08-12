using CMA.ISMAI.Delivery.Payment.Domain.Commands;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using Moq;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.Payment.Domain.Tests
{
    public class NotifyActionCommandHandler_Tests
    {
        [Fact(DisplayName = "Fail to Send notification")]
        [Trait("NotifyActionCommandHandler", "Send notification")]
        public void VerifyIfThePaymentInTheFile_Sucess()
        {
            // Arrange
            var deliveryFile = new NotifyActionCommand("A029216", "ISMAI", "Informática", "a029216@ismai.pt", "Hello World");
            var notifyService = new Mock<INotifyService>();

            notifyService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var deliveryFileHandler = new NotifyActionCommandHandler(notifyService.Object);

            // Act
            var result = deliveryFileHandler.Handle(deliveryFile, new CancellationToken());

            // Assert
            Assert.False(result.Result.IsValid);
        }

        [Fact(DisplayName = "Send notification")]
        [Trait("NotifyActionCommandHandler", "Verify payment of the delivery")]
        public void VerifyIfThePaymentInTheFile_Fail()
        {
            // Arrange
            var deliveryFile = new NotifyActionCommand("A029216", "ISMAI", "Informática", "a029216@ismai.pt", "Hello World");
            var notifyService = new Mock<INotifyService>();

            notifyService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var deliveryFileHandler = new NotifyActionCommandHandler(notifyService.Object);

            // Act
            var result = deliveryFileHandler.Handle(deliveryFile, new CancellationToken());
            Assert.True(result.Result.IsValid);
        }
    }
}
