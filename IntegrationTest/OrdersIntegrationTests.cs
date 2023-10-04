using Newtonsoft.Json;
using System.Text;
using UnitTestExemple.Application;
using UnitTestExemple.Data;
using UnitTestExemple.Domain.Entities;

namespace IntegrationTesting;

public class OrdersIntegrationTests : IClassFixture<OrdersApiFactory>
{
    private readonly OrdersApiFactory _ordersApiFactory;

    public OrdersIntegrationTests(OrdersApiFactory ordersApiFactory)
    {
        _ordersApiFactory = ordersApiFactory;
    }

    [Fact]
    public async Task Create_Order_Correctly()
    {
        // Arrange
        var sut = _ordersApiFactory.CreateClient();

        // Act
        var actual = await CreateOrderAsync(sut);
        
        // Assert
        var order = await ReadOrderFromResponse(actual);
        Assert.Equal(System.Net.HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(OrderStatus.Submitted, order.Status);
    }

    [Fact]
    public async Task Update_Order_Status_to_Paid()
    {
        // Arrange
        var order = new Order();
        var context = new AppDbContext();
        context.Orders.Add(order);
        var sut = _ordersApiFactory.CreateClient();

        // Act
        var actual = await sut.PutAsync($"api/order/{order.Id}/pay", null);
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, actual.StatusCode);
        var actualOrder = await ReadOrderFromResponse(actual);
        Assert.Equal(OrderStatus.Paid, actualOrder.Status);
    }

    private static async Task<HttpResponseMessage> CreateOrderAsync(HttpClient sut)
    {
        var orderItems = new[]
        {
            new OrderItemRequest(ProductName: "chair", UnitPrice: 150, Discount: 10, Units: 3)
        };

        var body = new OrderRequest(orderItems);

        var requestContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        return await sut.PostAsync("/api/order", requestContent);
    }

    private static async Task<Order> ReadOrderFromResponse(HttpResponseMessage response)
    {
        var contents = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Order>(contents) ?? new Order();
    }
}