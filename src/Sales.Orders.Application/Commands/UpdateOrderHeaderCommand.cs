using MediatR;
using Sales.Orders.Application.Common;

namespace Sales.Orders.Application.Commands;

public record UpdateOrderHeaderCommand(
    Guid OrderId,
    Guid CompanyId,
    string CompanyName,
    Guid CustomerId,
    string CustomerName)
    : IRequest<Result<bool>>;