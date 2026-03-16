using Sales.Orders.Domain.Common;

namespace Sales.Orders.Domain.ValueObjects;

public sealed record Customer
{
    public Guid Id { get; }
    public string Name { get; }

    private Customer(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static (Customer? Value, List<Notification> Errors) Create(Guid id, string name)
    {
        var errors = new List<Notification>();

        if (id == Guid.Empty)
            errors.Add(new Notification(nameof(Id), "Customer Id cannot be empty."));

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new Notification(nameof(Name), "Customer name is required."));

        return errors.Count != 0 ? (null, errors) : (new Customer(id, name), errors);
    }
}