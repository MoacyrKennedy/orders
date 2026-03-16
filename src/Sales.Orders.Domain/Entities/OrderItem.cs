using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Product Product { get; private set; } = null!;
    public double Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }

    private OrderItem()
    {
    }

    public OrderItem(Product product, double quantity, decimal unitPrice, decimal discount)
    {
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        RecalculateTotal();
    }

    internal void ApplyDiscount(decimal discount)
    {
        Discount = discount;
        RecalculateTotal();
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
        RecalculateTotal();
    }

    internal void UpdateQuantity(double quantity)
    {
        Quantity = quantity;
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        Total = UnitPrice * (decimal)Quantity - Discount;
    }
}