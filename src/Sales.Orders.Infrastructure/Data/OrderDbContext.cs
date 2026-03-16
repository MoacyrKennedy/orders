using Microsoft.EntityFrameworkCore;
using Sales.Orders.Domain.Common;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Infrastructure.Data.Configurations;

namespace Sales.Orders.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        modelBuilder.Ignore<Notification>();
        modelBuilder.Ignore<IDomainEvent>();

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}