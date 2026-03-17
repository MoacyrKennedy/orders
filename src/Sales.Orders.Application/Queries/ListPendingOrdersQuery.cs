using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Interfaces;

namespace Sales.Orders.Application.Queries;

public record ListPendingOrdersQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<List<PendingOrderDto>>>;

public record PendingOrderDto(
    Guid Id,
    int Number,
    Guid CompanyId,
    string CompanyName,
    Guid CustomerId,
    string CustomerName,
    decimal Total,
    int ItemsCount,
    DateTime CreatedAt);

public sealed class ListPendingOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<ListPendingOrdersQuery, Result<List<PendingOrderDto>>>
{
    public async Task<Result<List<PendingOrderDto>>> Handle(ListPendingOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

        var orders = await orderRepository.ListPendingAsync(page, pageSize);

        var response = orders.Select(order => new PendingOrderDto(
            order.Id,
            order.Number,
            order.Company.Id,
            order.Company.Name,
            order.Customer.Id,
            order.Customer.Name,
            order.Total,
            order.Items.Count,
            order.CreatedAt)).ToList();

        return Result<List<PendingOrderDto>>.Success(response);
    }
}
