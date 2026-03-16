using MediatR;
using Sales.Orders.Application.Common;

namespace Sales.Orders.Application.Commands;

public record CreateOrderCommand(
    Guid CompanyId,
    string CompanyName,
    Guid CustomerId,
    string CustomerName,
    List<CreateOrderItemCommand> Items)
    : IRequest<Result<Guid>>;

public record CreateOrderItemCommand(
    Guid ProductId,
    string ProductName,
    double Quantity,
    decimal UnitPrice,
    decimal Discount)
    : IRequest;