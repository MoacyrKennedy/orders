using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Commands;

public record CancelOrderCommand(Guid OrderId)
    : IRequest<Result<bool>>;

public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(request.OrderId);

        if (order == null)
            return Result<bool>.Failure("Order not found.");

        if (order.OrderStatus is OrderStatus.Completed or OrderStatus.Cancelled)
            return Result<bool>.Failure("Order cannot be cancelled in the current status.");

        order.Cancel();

        orderRepository.Update(order);
        await unitOfWork.CommitAsync();

        return Result<bool>.Success(true);
    }
}
