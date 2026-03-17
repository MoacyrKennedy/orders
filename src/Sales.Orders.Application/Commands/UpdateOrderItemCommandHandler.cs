using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Commands;

public sealed class UpdateOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrderItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            return Result<bool>.Failure("Quantity must be greater than zero.");

        if (request.UnitPrice <= 0)
            return Result<bool>.Failure("UnitPrice must be greater than zero.");

        if (request.Discount < 0)
            return Result<bool>.Failure("Discount cannot be negative.");

        var order = await orderRepository.FindByIdAsync(request.OrderId);

        if (order == null)
            return Result<bool>.Failure("Order not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            return Result<bool>.Failure("Only pending orders can have items changed.");

        var item = order.Items.FirstOrDefault(i => i.Id == request.ItemId && !i.IsDeleted);
        if (item == null)
            return Result<bool>.Failure("Item not found.");

        order.UpdateQuantityItem(request.ItemId, request.Quantity);
        order.UpdateUnitPriceItem(request.ItemId, request.UnitPrice);
        order.ApplyDiscountItem(request.ItemId, request.Discount);

        orderRepository.Update(order);
        await unitOfWork.CommitAsync();

        return Result<bool>.Success(true);
    }
}
