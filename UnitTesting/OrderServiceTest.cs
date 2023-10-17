using Microsoft.EntityFrameworkCore.Storage;
using NSubstitute;
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
    
            var mockRepository = Substitute.For<IOrderRepository>();
            var sut = new OrderService(mockRepository);

            mockRepository.GetAllOrdersAsync().Returns(expectedOrders);

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
            var mockRepository = Substitute.For<IOrderRepository>();
            var sut = new OrderService(mockRepository);

            var mockTransaction = Substitute.For<IDbContextTransaction>();
            mockRepository.BeginTransactionAsync().Returns(mockTransaction);
            mockRepository.GetOrderAsync(Arg.Any<Guid>()).Returns(order);

            // Act
            var actual = await sut.SetOrderAsPaid(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Paid, actual.Status);
        }
    }
}