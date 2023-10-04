using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MockQueryable.Moq;
using UnitTestExemple.Data;
using UnitTestExemple.Domain.Entities;
using UnitTestExemple.Domain.Services;

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

            var mockSet = expectedOrders.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            var sut = new OrderService(mockContext.Object);

            // Act
            var actual = await sut.GetOrdersAsync();

            // Assert
            Assert.Equal(expectedTotalOrders, actual.Count());
            Assert.Equal(expectedOrders.FirstOrDefault(), actual.FirstOrDefault());
        }

        [Fact]
        public async Task Orders_correctly()
        {
            // Arrange 
            var order = new Order()
            {
                Items = new[]
                {
                    new OrderItem("pencil", 14, 0, 30)
                }
            };
            var orders = new[]
            {
                order
            };

            var mockContext = new Mock<AppDbContext>();
             
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockContext.Setup(c => c.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            
            var mockSet = orders.AsQueryable().BuildMockDbSet();
            mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            var sut = new OrderService(mockContext.Object);

            // Act
            var actual = await sut.SetOrderAsPaid(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Paid, actual.Status);
        }
    }
}