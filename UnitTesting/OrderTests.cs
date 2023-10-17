using UnitTestExemple.Domain.Entities;

namespace UnitTesting;

public class OrderTests
{
    [Fact]
    public void Order_AddOneItem_ShouldBeOk()
    {
        // Arrange
        var expected = 1;
        var sut = new Order();
        var item = new OrderItem("productName", 1, 1, 1);

        // Act
        sut.AddItem(item);
            
        // Assert
        Assert.NotEmpty(sut.Items);
        Assert.Equal(sut.Items.Count, expected);
        var actual = sut.Items.FirstOrDefault();
        Assert.Equal(item.ProductName, actual.ProductName);
        Assert.Equal(item.UnitPrice, actual.UnitPrice);
        Assert.Equal(item.Units, actual.Units);
        Assert.Equal(item.UnitPrice, actual.UnitPrice);
    }
}