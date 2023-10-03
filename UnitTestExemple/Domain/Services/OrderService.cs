using Microsoft.EntityFrameworkCore;
using UnitTestExemple.Data;
using UnitTestExemple.Domain.Entities;

namespace UnitTestExemple.Domain.Services;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await _context.Orders.AsNoTracking().ToListAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order> SetOrderAsPaid(Guid orderId)
    {
        var order = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId) ?? throw new Exception($"Order {orderId} not found");

        var transaction = await _context.BeginTransactionAsync();

        order.SetAsPaid();

        _context.SaveChanges();

        await transaction.CommitAsync();

        return order;
    }
}