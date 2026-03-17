using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Entities;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Application.Commands;

public sealed class AddOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<AddOrderItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            return Result<bool>.Failure("Quantity must be greater than zero.");

        if (request.UnitPrice <= 0)
            return Result<bool>.Failure("UnitPrice must be greater than zero.");

        if (request.Discount < 0)
            return Result<bool>.Failure("Discount cannot be negative.");

        if (string.IsNullOrWhiteSpace(request.ProductName))
            return Result<bool>.Failure("ProductName is required.");

        var order = await orderRepository.FindByIdAsync(request.OrderId);

        if (order == null)
            return Result<bool>.Failure("Order not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            return Result<bool>.Failure("Only pending orders can receive new items.");

        var item = new OrderItem(
            new Product(request.ProductId, request.ProductName),
            request.Quantity,
            request.UnitPrice,
            request.Discount);

        order.AddItem(item);

        orderRepository.Update(order);

        await unitOfWork.CommitAsync();

        return Result<bool>.Success(true);
    }
}
