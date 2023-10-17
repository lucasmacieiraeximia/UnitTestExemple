using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using UnitTestExemple.Application;
using UnitTestExemple.Data;
using UnitTestExemple.Domain.Entities;

namespace IntegrationTesting;

public class OrdersIntegrationTests : IClassFixture<OrdersApiFactory>
{
    private readonly OrdersApiFactory _ordersApiFactory;
    private readonly AppDbContext _context;

    public OrdersIntegrationTests(OrdersApiFactory ordersApiFactory)
    {
        _ordersApiFactory = ordersApiFactory;
        _context = DbContextFactory.NewContext();
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
        var sut = _ordersApiFactory.CreateClient();

        var order = new Order();
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act
        var actual = await sut.PutAsync($"api/order/{order.Id}/pay", null);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, actual.StatusCode);
        var actualOrder = await ReadOrderFromResponse(actual);
        Assert.Equal(OrderStatus.Paid, actualOrder.Status);
        AssertDatabase(_context, actualOrder.Id);
        
        // Teardown
        _context.Dispose();
    }

    private static void AssertDatabase(AppDbContext context, Guid actualOrderId)
    {
        var value = context.Orders.AsNoTracking().SingleOrDefault(order => order.Id == actualOrderId);

        Assert.NotNull(value);
        Assert.Equal(OrderStatus.Paid, value.Status);
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

public class DbContextFactory
{
    public static AppDbContext NewContext()
    {
        const int HostPort = 3307;
        const string ContainerPassword = "my-secret-pw";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(
                $"server=localhost;port={HostPort};database=unittetstexample;uid=root;password={ContainerPassword}",
                new MySqlServerVersion(new Version(8, 0, 34))
            );

        return new AppDbContext(optionsBuilder.Options);
    }
}