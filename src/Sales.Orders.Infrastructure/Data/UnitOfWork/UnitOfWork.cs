using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Infrastructure.Data.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;

    public UnitOfWork(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}