using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Test.Domain.Entities;

public class OrderTest
{
    private readonly Order _order = new(Customer.Create(Guid.NewGuid(), "Customer Test").Value!,
        Company.Create(Guid.NewGuid(), "Company Test").Value!, []);

    [Fact(DisplayName = "must create order number")]
    public void T1()
    {
        Assert.True(_order.Number > 0);
    }

    [Fact(DisplayName = "The initial order status should be pending.")]
    public void T2()
    {
        Assert.Equal(OrderStatus.Pending, _order.OrderStatus);
    }

    [Fact(DisplayName = "must calculate the order total when created with items.")]
    public void T2_1()
    {
        var order = new Order(
            Customer.Create(Guid.NewGuid(), "Customer Test").Value!,
            Company.Create(Guid.NewGuid(), "Company Test").Value!,
            [
                new OrderItem(new Product(Guid.NewGuid(), "Product A"), 2, 10m, 1m),
                new OrderItem(new Product(Guid.NewGuid(), "Product B"), 1, 5m, 0m)
            ]);

        Assert.Equal(24m, order.Total);
    }

    [Fact(DisplayName = "must calculate the order total when adding an item.")]
    public void T3()
    {
        _order.AddItem(new OrderItem(new Product(Guid.NewGuid(), "Product Test"), 2.0, 1.99m, 1.0m));
        Assert.Equal(2.98m, _order.Total);
    }

    [Fact(DisplayName = "must calculate the order total when removing an item.")]
    public void T4()
    {
        _order.AddItem(new OrderItem(new Product(Guid.NewGuid(), "Product Test"), 2.0, 1.99m, 1.0m));
        var item = _order.Items.Last();
        _order.RemoveItem(item.Id);
        Assert.True(_order.Items.Last().IsDeleted);
        Assert.Equal(0m, _order.Total);
    }

    [Fact(DisplayName = "should apply a discount to the item.")]
    public void T5()
    {
        _order.AddItem(new OrderItem(new Product(Guid.NewGuid(), "Product Test"), 2.0, 1.99m, 1.0m));
        var itemId = _order.Items.Last().Id;
        _order.ApplyDiscountItem(itemId, 0.99m);
        Assert.Equal(2.99m, _order.Items.Last().Total);
    }

    [Fact(DisplayName = "The unit price of the item must be changed.")]
    public void T6()
    {
        _order.AddItem(new OrderItem(new Product(Guid.NewGuid(), "Product Test"), 2.0, 1.99m, 0m));
        var itemId = _order.Items.Last().Id;
        _order.UpdateUnitPriceItem(itemId, 1.00m);
        Assert.Equal(2.00m, _order.Items.Last().Total);
    }

    [Fact(DisplayName = "must change the quantity of the item.")]
    public void T7()
    {
        _order.AddItem(new OrderItem(new Product(Guid.NewGuid(), "Product Test"), 2.0, 1.99m, 0m));
        var itemId = _order.Items.Last().Id;
        _order.UpdateQuantityItem(itemId, 3);
        Assert.Equal(5.97m, _order.Items.Last().Total);
    }


    [Fact(DisplayName = "must remove the order and the order item together.")]
    public void T8()
    {
        _order.AddItem(new OrderItem(new Product(Guid.NewGuid(), "Product Test"), 2.0, 1.99m, 0m));
        _order.SoftDelete();
        Assert.True(_order.Items.Last().IsDeleted);
        Assert.True(_order.IsDeleted);
    }

    [Fact(DisplayName = "The entity will become invalid when updating the header with null parameters.")]
    public void T9()
    {
        _order.UpdateHeader(null!, null!);
        Assert.False(_order.IsValid);
    }

    [Fact(DisplayName = "must cancel order when status allows cancellation")]
    public void T10()
    {
        _order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, _order.OrderStatus);
    }
}