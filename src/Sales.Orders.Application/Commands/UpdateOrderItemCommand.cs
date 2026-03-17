using MediatR;
using Sales.Orders.Application.Common;

namespace Sales.Orders.Application.Commands;

public record UpdateOrderItemCommand(
    Guid OrderId,
    Guid ItemId,
    double Quantity,
    decimal UnitPrice,
    decimal Discount)
    : IRequest<Result<bool>>;
