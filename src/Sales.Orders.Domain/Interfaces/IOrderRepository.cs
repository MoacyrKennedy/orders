using Sales.Orders.Domain.Entities;

namespace Sales.Orders.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> FindByIdAsync(Guid id);
    void Update(Order entity);
}