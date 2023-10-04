using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UnitTestExemple.Domain.Entities;

namespace UnitTestExemple.Data.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext appDbContext) {
        _context = appDbContext;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.BeginTransactionAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetOrderAsync(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(order => order.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.AsNoTracking().ToListAsync();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
