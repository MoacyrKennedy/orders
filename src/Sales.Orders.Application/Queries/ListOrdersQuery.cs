using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Enums;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Queries;

public record ListOrdersQuery(
    OrderStatus? Status,
    Guid? CompanyId,
    DateTime? CreatedDate,
    int Page = 1,
    int PageSize = 20)
    : IRequest<Result<List<OrderSummaryDto>>>;

public record OrderSummaryDto(
    Guid Id,
    int Number,
    Guid CompanyId,
    string CompanyName,
    Guid CustomerId,
    string CustomerName,
    OrderStatus Status,
    decimal Total,
    int ItemsCount,
    DateTime CreatedAt);

public sealed class ListOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<ListOrdersQuery, Result<List<OrderSummaryDto>>>
{
    public async Task<Result<List<OrderSummaryDto>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

        var orders = await orderRepository.ListAsync(request.Status, request.CompanyId, request.CreatedDate, page, pageSize);

        var response = orders.Select(order => new OrderSummaryDto(
            order.Id,
            order.Number,
            order.Company.Id,
            order.Company.Name,
            order.Customer.Id,
            order.Customer.Name,
            order.OrderStatus,
            order.Total,
            order.Items.Count(i => !i.IsDeleted),
            order.CreatedAt)).ToList();

        return Result<List<OrderSummaryDto>>.Success(response);
    }
}
