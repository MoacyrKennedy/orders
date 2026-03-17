using MediatR;
using Sales.Orders.Application.Common;

namespace Sales.Orders.Application.Commands;

public record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    double Quantity,
    decimal UnitPrice,
    decimal Discount)
    : IRequest<Result<bool>>;
