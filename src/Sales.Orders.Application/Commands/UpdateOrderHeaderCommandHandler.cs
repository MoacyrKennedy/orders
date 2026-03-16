using MediatR;
using Sales.Orders.Application.Common;
using Sales.Orders.Domain.Interfaces;
using Sales.Orders.Domain.ValueObjects;

namespace Sales.Orders.Application.Commands;

public sealed class UpdateOrderHeaderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrderHeaderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderHeaderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(request.OrderId);

        if (order == null)
            return Result<bool>.Failure("Order not found.");
        
        var (company, companyErrors) = Company.Create(request.CompanyId, request.CompanyName);
        var (customer, customerErrors) = Customer.Create(request.CustomerId, request.CustomerName);

        var allErrors = companyErrors.Concat(customerErrors).ToList();
        if (allErrors.Count != 0)
        {
            var messages = string.Join(", ", allErrors.Select(e => e.Message));
            return Result<bool>.Failure(messages);
        }

        order.UpdateHeader(company: company!, customer: customer!);

        orderRepository.Update(order);

        await unitOfWork.CommitAsync();

        return Result<bool>.Success(true);
    }
}