using CMA.ISMAI.Delivery.Payment.Domain.Commands;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using CMA.ISMAI.Delivery.Payment.Domain.Model;
using Moq;
using NetDevPack.Mediator;
using System.Threading;
using Xunit;

namespace CMA.ISMAI.Delivery.Payment.Domain.Tests
{
    public class DeliveryPaymentHandler_Tests
    {
        [Fact(DisplayName = "Not paid")]
        [Trait("DeliveryPaymentHandler", "Verify payment of the delivery")]
        public void VerifyIfThePaymentInTheFile_Sucess()
        {
            // Arrange
            var deliveryFile = new VerifyPaymentOfDeliveryCommand("A029216", "ISMAI", "Informática");
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();

            fileReader.Setup(x => x.PaymentHasBeenDone(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var deliveryFileHandler = new DeliveryPaymentHandler(fileReader.Object, meditrHandler.Object);

            // Act
            var result = deliveryFileHandler.Handle(deliveryFile, new CancellationToken());

            // Assert
            Assert.True(result.Result.IsValid);
        }

        [Fact(DisplayName = "Student paid")]
        [Trait("DeliveryPaymentHandler", "Verify payment of the delivery")]
        public void VerifyIfThePaymentInTheFile_Fail()
        {
            // Arrange
            var deliveryFile = new VerifyPaymentOfDeliveryCommand("A029216", "ISMAI", "Informática");
            var fileReader = new Mock<IFileReaderService>();
            var meditrHandler = new Mock<IMediatorHandler>();
            fileReader.Setup(x => x.PaymentHasBeenDone(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            //Act
            var deliveryFileHandler = new DeliveryPaymentHandler(fileReader.Object, meditrHandler.Object);
            // Assert
            var result = deliveryFileHandler.Handle(deliveryFile, new CancellationToken());
            Assert.False(result.Result.IsValid);
        }
    }
}
