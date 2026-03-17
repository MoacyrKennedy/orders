using Microsoft.EntityFrameworkCore;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;
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

    public async Task<List<Order>> ListPendingAsync(int page, int pageSize)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.OrderStatus == OrderStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Order>> ListAsync(OrderStatus? status, Guid? companyId, DateTime? createdDate, int page,
        int pageSize)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.OrderStatus == status.Value);

        if (companyId.HasValue)
            query = query.Where(x => x.Company.Id == companyId.Value);

        if (createdDate.HasValue)
        {
            var start = createdDate.Value.Date;
            var end = start.AddDays(1);
            query = query.Where(x => x.CreatedAt >= start && x.CreatedAt < end);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public void Update(Order entity)
    {
        _context.Orders.Update(entity);
    }
}