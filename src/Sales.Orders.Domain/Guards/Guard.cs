using Sales.Orders.Domain.Entities;

namespace Sales.Orders.Domain.Guards;

public static class Guard
{
    public static void NotNull<T>(T? value, string name, BaseEntity entity) where T : class
    {
        if (value is null)
            entity.AddNotification(name, $"{name} is required");
    }

    public static void NotNullOrEmpty(string value, string name, BaseEntity entity)
    {
        if (string.IsNullOrWhiteSpace(value))
            entity.AddNotification(name, $"{name} cannot be empty");
    }
}