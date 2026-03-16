using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Orders.Domain.Entities;

namespace Sales.Orders.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).IsRequired();

        builder.OwnsOne(x => x.Company, owner =>
        {
            owner.Property(x => x.Name).IsRequired().HasMaxLength(100);
            owner.Property(x => x.Id).IsRequired();
        });
        
        builder.OwnsOne(x => x.Customer, owner =>
        {
            owner.Property(x => x.Name).IsRequired().HasMaxLength(100);
            owner.Property(x => x.Id).IsRequired();
        });
        
        builder.Property(x => x.OrderStatus).IsRequired();
        builder.Property(x => x.Total).HasPrecision(18, 2);
        
        builder.OwnsMany(x => x.Items, oi =>
        {
            oi.WithOwner().HasForeignKey("OrderId");
            oi.HasKey(x => x.Id);
            oi.Property(x => x.Id).ValueGeneratedNever();
            
            oi.OwnsOne(x => x.Product, owner =>
            {
                owner.Property(x => x.Id).IsRequired();
                owner.Property(x => x.Name).IsRequired().HasMaxLength(100);
            });
            
            oi.Property(x => x.Quantity).IsRequired();
            oi.Property(x => x.UnitPrice).HasPrecision(18, 2);
            oi.Property(x => x.Discount).HasPrecision(18, 2);
            oi.Property(x => x.Total).HasPrecision(18, 2);
        });
    }
}