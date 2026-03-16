namespace Sales.Orders.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}