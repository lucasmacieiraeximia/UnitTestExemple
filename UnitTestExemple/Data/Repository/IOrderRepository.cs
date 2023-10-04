using Microsoft.EntityFrameworkCore.Storage;
using UnitTestExemple.Domain.Entities;

namespace UnitTestExemple.Data.Repository;

public interface IOrderRepository
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task AddOrderAsync(Order order);
    Task<Order?> GetOrderAsync(Guid orderId);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    void SaveChanges();
}
