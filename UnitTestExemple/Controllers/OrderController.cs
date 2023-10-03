using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using UnitTestExemple.Application;
using UnitTestExemple.Domain.Entities;
using UnitTestExemple.Domain.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UnitTestExemple.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService orderService;
    private readonly ILogger<OrderController> logger;

    public OrderController(ILogger<OrderController> logger, OrderService orderService)
    {
        this.orderService = orderService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await orderService.GetOrdersAsync();
    }

    [HttpPost]
    public async Task<Order> Post([FromBody] OrderRequest request)
    {
        var order = new Order();

        foreach (var item in request.Items)
        {
            order.AddItem(new OrderItem(item.ProductName, item.UnitPrice, item.Discount, item.Units));
        }

        await orderService.AddOrderAsync(order);

        logger.LogInformation("Order with Id {OrderId} created", order.Id);

        var meter = new Meter(name: "outbox");
        var counter = meter.CreateCounter<int>("orders.counting", "Number of orders created");
        counter.Add(1);

        return order;
    }

    [HttpPut]
    [Route("{orderId}/pay")]
    public async Task<Order> Put(Guid orderId)
    {
        var order = await orderService.SetOrderAsPaid(orderId);

        return order;
    }
}
