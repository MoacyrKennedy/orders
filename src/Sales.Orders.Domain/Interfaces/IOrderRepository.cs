using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;

namespace Sales.Orders.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> FindByIdAsync(Guid id);
    Task<List<Order>> ListPendingAsync(int page, int pageSize);
    Task<List<Order>> ListAsync(OrderStatus? status, Guid? companyId, DateTime? createdDate, int page, int pageSize);
    void Update(Order entity);
}