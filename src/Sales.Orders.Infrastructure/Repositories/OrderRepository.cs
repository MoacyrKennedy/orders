using Microsoft.EntityFrameworkCore;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Infrastructure.Data;

namespace Sales.Orders.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> FindByIdAsync(Guid id)
    {
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Update(Order entity)
    {
        _context.Orders.Update(entity);
    }
}