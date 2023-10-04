using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using UnitTestExemple.Domain.Entities;
using UnitTestExemple.Domain.Services;
using UnitTestExemple.Data.Repository;

namespace UnitTesting
{
    public class OrderServiceTest
    {
        [Fact]
        public async Task List_Orders_Should_Be_Ok()
        {
            // Arrange
            var expectedTotalOrders = 1;
            var expectedOrders = new[]
            {
                new Order()
            };

            var mockRepository = new Mock<IOrderRepository>();
            var sut = new OrderService(mockRepository.Object);

            mockRepository.Setup(r => r.GetAllOrdersAsync()).ReturnsAsync(expectedOrders);

            // Act
            var actual = await sut.GetOrdersAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedTotalOrders, actual.Count());
        }

        [Fact]
        public async Task Order_Status_Should_Change_to_Paid()
        {
            // Arrange 
            var order = new Order();

            var mockRepository = new Mock<IOrderRepository>();
            var sut = new OrderService(mockRepository.Object);

            var mockTransaction = new Mock<IDbContextTransaction>();
            mockRepository.Setup(c => c.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            mockRepository.Setup(c => c.GetOrderAsync(order.Id)).ReturnsAsync(order);

            // Act
            var actual = await sut.SetOrderAsPaid(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Paid, actual.Status);
        }
    }
}