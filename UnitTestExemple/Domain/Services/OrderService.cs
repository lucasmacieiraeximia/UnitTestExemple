using UnitTestExemple.Data.Repository;
using UnitTestExemple.Domain.Entities;

namespace UnitTestExemple.Domain.Services;

public class OrderService
{
    private readonly IOrderRepository orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        this.orderRepository = orderRepository;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await orderRepository.GetAllOrdersAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        await orderRepository.AddOrderAsync(order);
    }

    public async Task<Order> SetOrderAsPaid(Guid orderId)
    {
        var transaction = await orderRepository.BeginTransactionAsync();

        var order = await orderRepository.GetOrderAsync(orderId) ?? throw new Exception($"Order {orderId} not found");

        order.SetAsPaid();

        orderRepository.SaveChanges();

        await transaction.CommitAsync();

        return order;
    }
}