using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Commands;

public record RemoveOrderItemCommand(Guid OrderId, Guid ItemId)
    : IRequest<Result<bool>>;

public sealed class RemoveOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveOrderItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(request.OrderId);

        if (order == null)
            return Result<bool>.Failure("Order not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            return Result<bool>.Failure("Only pending orders can have items removed.");

        var item = order.Items.FirstOrDefault(i => i.Id == request.ItemId && !i.IsDeleted);
        if (item == null)
            return Result<bool>.Failure("Item not found.");

        order.RemoveItem(request.ItemId);

        orderRepository.Update(order);
        await unitOfWork.CommitAsync();

        return Result<bool>.Success(true);
    }
}
