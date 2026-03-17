using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Queries;

public record GetOrderByIdQuery(Guid Id)
    : IRequest<Result<OrderDetailsDto>>;

public record OrderItemDetailsDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    double Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Total);

public record OrderDetailsDto(
    Guid Id,
    int Number,
    Guid CompanyId,
    string CompanyName,
    Guid CustomerId,
    string CustomerName,
    OrderStatus Status,
    decimal Total,
    DateTime CreatedAt,
    List<OrderItemDetailsDto> Items);

public sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailsDto>>
{
    public async Task<Result<OrderDetailsDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(request.Id);

        if (order == null)
            return Result<OrderDetailsDto>.Failure("Order not found.");

        var response = new OrderDetailsDto(
            order.Id,
            order.Number,
            order.Company.Id,
            order.Company.Name,
            order.Customer.Id,
            order.Customer.Name,
            order.OrderStatus,
            order.Total,
            order.CreatedAt,
            order.Items
                .Where(i => !i.IsDeleted)
                .Select(i => new OrderItemDetailsDto(
                    i.Id,
                    i.Product.Id,
                    i.Product.Name,
                    i.Quantity,
                    i.UnitPrice,
                    i.Discount,
                    i.Total))
                .ToList());

        return Result<OrderDetailsDto>.Success(response);
    }
}
