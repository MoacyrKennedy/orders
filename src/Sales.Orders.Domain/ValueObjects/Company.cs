using Sales.Orders.Domain.Common;

namespace Sales.Orders.Domain.ValueObjects;

public sealed record Company
{
    public Guid Id { get; }
    public string Name { get; }

    private Company(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static (Company? Value, List<Notification> Errors) Create(Guid id, string name)
    {
        var errors = new List<Notification>();

        if (id == Guid.Empty)
            errors.Add(new Notification(nameof(Id), "Company Id cannot be empty."));

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new Notification(nameof(Name), "Company name is required."));

        return errors.Count != 0 ? (null, errors) : (new Company(id, name), errors);
    }
}
