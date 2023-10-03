using Newtonsoft.Json;
using System.Text;
using UnitTestExemple.Application;
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
        var result = await CreateOrderAsync(sut);
        var order = await GetOrderFromResponse(result);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(OrderStatus.Submitted, order.Status);
    }

    [Fact]
    public async Task Update_Order_Status_to_Paid()
    {
        // Arrange
        var sut = _ordersApiFactory.CreateClient();

        // Act
        var result = await CreateOrderAsync(sut);
        var order = await GetOrderFromResponse(result); 

        result = await sut.PutAsync($"api/order/{order.Id}/pay", null);
        order = await GetOrderFromResponse(result);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    private static async Task<HttpResponseMessage> CreateOrderAsync(HttpClient sut)
    {
        var orderItems = new[]
                {
            new OrderItemRequest(ProductName: "chair", UnitPrice: 150, Discount: 10, Units: 3)
        };

        var body = new OrderRequest(orderItems);

        var requestContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        var result = await sut.PostAsync("/api/order", requestContent);

        return result;
    }


    private static async Task<Order> GetOrderFromResponse(HttpResponseMessage response)
    {
        var contents = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Order>(contents) ?? new Order();
    }
}