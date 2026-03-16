using Sales.Orders.Domain.Common;

namespace Sales.Orders.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; private set; }
    private readonly List<Notification> _notifications;
    private readonly List<IDomainEvent> _domainEvents;
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;
    public bool IsValid => !_notifications.Any(n => n.IsError);


    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
        _notifications = [];
        _domainEvents = [];
    }

    public void AddNotification(string key, string message, bool isError = true) =>
        _notifications.Add(new Notification(key, message, isError));

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public virtual void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}