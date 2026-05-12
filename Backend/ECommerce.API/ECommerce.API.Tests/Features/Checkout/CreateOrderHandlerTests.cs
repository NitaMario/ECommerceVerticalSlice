using ECommerce.API.Features.Checkout.CreateOrder;

namespace ECommerce.API.Tests.Features.Checkout
{
    public class CreateOrderHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCartIsEmpty_ReturnsFailureResult()
        {
            // Arrange
            var mockConnectionFactory = new Mock<ISqlConnectionFactory>();
            var mockConnection = new Mock<IDbConnection>();
            mockConnectionFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);
            var handler = new CreateOrderHandler(mockConnectionFactory.Object);
            var command = new CreateOrderCommand(UserId: 1, ShippingAddress: "123 Test St");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Cannot create order: Cart is empty.");
            result.OrderId.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WithValidCartItems_CreateOrderAndReturnsSuccess()
        {
            // Arrange
            var mockConnectionFactory = new Mock<ISqlConnectionFactory>();
            var mockConnection = new Mock<IDbConnection>();
            mockConnectionFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var handler = new CreateOrderHandler(mockConnectionFactory.Object);
            var command = new CreateOrderCommand(UserId: 1, ShippingAddress: "456 Test St");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Order created successfully.");
            result.OrderId.Should().BeGreaterThan(0);
        }
    }  
}
