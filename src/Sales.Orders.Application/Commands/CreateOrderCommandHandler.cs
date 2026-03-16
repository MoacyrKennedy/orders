using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Application.Commands;

public sealed class CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
            return Result<Guid>.Failure("The order must contain at least one item.");

        var items = request.Items.Select(i => new OrderItem(new Product(Id: i.ProductId, Name: i.ProductName),
            quantity: i.Quantity, unitPrice: i.UnitPrice, discount: i.Discount)).ToList();

        var order = new Order( Customer.Create(id: request.CustomerId, name: request.CustomerName).Value!,
            Company.Create(id: request.CompanyId, name: request.CompanyName).Value!, orderItems: items);

        await orderRepository.AddAsync(order);

        await unitOfWork.CommitAsync();

        return Result<Guid>.Success(order.Id);
    }
}