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

        var (company, companyErrors) = Company.Create(request.CompanyId, request.CompanyName);
        var (customer, customerErrors) = Customer.Create(request.CustomerId, request.CustomerName);

        var allHeaderErrors = companyErrors.Concat(customerErrors).ToList();
        if (allHeaderErrors.Count != 0)
        {
            var messages = string.Join(", ", allHeaderErrors.Select(e => e.Message));
            return Result<Guid>.Failure(messages);
        }

        if (request.Items.Any(i => string.IsNullOrWhiteSpace(i.ProductName)))
            return Result<Guid>.Failure("All items must have a valid product name.");

        if (request.Items.Any(i => i.Quantity <= 0))
            return Result<Guid>.Failure("All items must have quantity greater than zero.");

        if (request.Items.Any(i => i.UnitPrice <= 0))
            return Result<Guid>.Failure("All items must have unit price greater than zero.");

        if (request.Items.Any(i => i.Discount < 0))
            return Result<Guid>.Failure("Discount cannot be negative.");

        var items = request.Items.Select(i => new OrderItem(new Product(Id: i.ProductId, Name: i.ProductName),
            quantity: i.Quantity, unitPrice: i.UnitPrice, discount: i.Discount)).ToList();

        var order = new Order(customer!, company!, orderItems: items);

        await orderRepository.AddAsync(order);

        await unitOfWork.CommitAsync();

        return Result<Guid>.Success(order.Id);
    }
}