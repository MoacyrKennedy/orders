using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Guards;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Domain.Entities;

public sealed class Order : BaseEntity
{
    public int Number { get; private set; }
    public Company Company { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;
    public OrderStatus OrderStatus { get; private set; }
    public decimal Total { get; private set; }
    private readonly List<OrderItem> _items;
    public IReadOnlyCollection<OrderItem> Items => _items;

    private Order()
    {
        _items = [];
    }

    public Order(Customer customer, Company company, List<OrderItem> orderItems) : this()
    {
        Number = new Random().Next(99999);
        Company = company;
        Customer = customer;
        _items = orderItems;
    }

    public void AddItem(OrderItem item)
    {
        if (IsNotePedding()) return;
        _items.Add(item);
        RecalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        if (IsNotePedding()) return;
        var index = _items.FindIndex(i => i.Id == itemId);
        _items[index].SoftDelete();
        RecalculateTotal();
    }

    public void ApplyDiscountItem(Guid itemId, decimal discount)
    {
        var index = _items.FindIndex(i => i.Id == itemId);
        _items[index].ApplyDiscount(discount);
    }

    public void UpdateUnitPriceItem(Guid itemId, decimal unitPrice)
    {
        var index = _items.FindIndex(i => i.Id == itemId);
        _items[index].UpdateUnitPrice(unitPrice);
    }

    public void UpdateQuantityItem(Guid itemId, double quantity)
    {
        var index = _items.FindIndex(i => i.Id == itemId);
        _items[index].UpdateQuantity(quantity);
    }

    private bool IsNotePedding()
    {
        return OrderStatus != OrderStatus.Pending;
    }

    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.Total);
    }

    public override void SoftDelete()
    {
        base.SoftDelete();
        _items.ForEach(i => i.SoftDelete());
    }

    public override string ToString()
    {
        return $"Number - {Number}, Company - {Company}, Customer - {Customer}";
    }

    public void UpdateHeader(Company company, Customer customer)
    {
        Guard.NotNull(company, nameof(company), this);
        Guard.NotNull(customer, nameof(customer), this);

        if (!IsValid) return;

        Company = company;
        Customer = customer;
    }
}