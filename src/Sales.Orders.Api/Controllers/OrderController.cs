using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.Orders.Application.Commands;
using Sales.Orders.Application.Queries;
using Sales.Orders.Domain.Enums;

namespace Sales.Orders.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public class OrderController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (string.Equals(result.Error, "Order not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = result.Error });

        return BadRequest(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] Guid? companyId,
        [FromQuery] DateTime? createdDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        OrderStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsed))
                return BadRequest(new { error = "Invalid status filter." });

            parsedStatus = parsed;
        }

        var result = await sender.Send(new ListOrdersQuery(parsedStatus, companyId, createdDate, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(
            request.CompanyId,
            request.CompanyName,
            request.CustomerId,
            request.CustomerName,
            request.Items.Select(i => new CreateOrderItemCommand(
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.Discount)).ToList());

        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Created($"/api/v1/orders/{result.Value}", new { id = result.Value });
    }

    [HttpPost("{orderId:guid}/items")]
    public async Task<IActionResult> AddItem([FromRoute] Guid orderId, [FromBody] AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddOrderItemCommand(
            orderId,
            request.ProductId,
            request.ProductName,
            request.Quantity,
            request.UnitPrice,
            request.Discount);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return Created($"/api/v1/orders/{orderId}/items", new { success = true });

        if (string.Equals(result.Error, "Order not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = result.Error });

        return BadRequest(new { error = result.Error });
    }

    [HttpPut("{orderId:guid}/header")]
    public async Task<IActionResult> UpdateHeader([FromRoute] Guid orderId, [FromBody] UpdateOrderHeaderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderHeaderCommand(
            orderId,
            request.CompanyId,
            request.CompanyName,
            request.CustomerId,
            request.CustomerName);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return Ok(new { success = true });

        if (string.Equals(result.Error, "Order not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = result.Error });

        return BadRequest(new { error = result.Error });
    }

    [HttpPut("{orderId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem([FromRoute] Guid orderId, [FromRoute] Guid itemId,
        [FromBody] UpdateOrderItemRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderItemCommand(orderId, itemId, request.Quantity, request.UnitPrice, request.Discount);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return Ok(new { success = true });

        if (string.Equals(result.Error, "Order not found.", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(result.Error, "Item not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = result.Error });

        return BadRequest(new { error = result.Error });
    }

    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid orderId, [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveOrderItemCommand(orderId, itemId), cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        if (string.Equals(result.Error, "Order not found.", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(result.Error, "Item not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = result.Error });

        return BadRequest(new { error = result.Error });
    }

    [HttpPut("{orderId:guid}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CancelOrderCommand(orderId), cancellationToken);

        if (result.IsSuccess)
            return Ok(new { success = true });

        if (string.Equals(result.Error, "Order not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = result.Error });

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("pending")]
    public async Task<IActionResult> ListPending([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new ListPendingOrdersQuery(page, pageSize), cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    public sealed class CreateOrderRequest
    {
        [Required] public Guid CompanyId { get; set; }
        [Required, MaxLength(100)] public string CompanyName { get; set; } = string.Empty;
        [Required] public Guid CustomerId { get; set; }
        [Required, MaxLength(100)] public string CustomerName { get; set; } = string.Empty;
        [Required, MinLength(1)] public List<CreateOrderItemRequest> Items { get; set; } = [];
    }

    public sealed class CreateOrderItemRequest
    {
        [Required] public Guid ProductId { get; set; }
        [Required, MaxLength(100)] public string ProductName { get; set; } = string.Empty;
        [Range(0.0000001, double.MaxValue)] public double Quantity { get; set; }
        [Range(typeof(decimal), "0.01", "999999999999.99")] public decimal UnitPrice { get; set; }
        [Range(typeof(decimal), "0", "999999999999.99")] public decimal Discount { get; set; }
    }

    public sealed class AddOrderItemRequest
    {
        [Required] public Guid ProductId { get; set; }
        [Required, MaxLength(100)] public string ProductName { get; set; } = string.Empty;
        [Range(0.0000001, double.MaxValue)] public double Quantity { get; set; }
        [Range(typeof(decimal), "0.01", "999999999999.99")] public decimal UnitPrice { get; set; }
        [Range(typeof(decimal), "0", "999999999999.99")] public decimal Discount { get; set; }
    }

    public sealed class UpdateOrderHeaderRequest
    {
        [Required] public Guid CompanyId { get; set; }
        [Required, MaxLength(100)] public string CompanyName { get; set; } = string.Empty;
        [Required] public Guid CustomerId { get; set; }
        [Required, MaxLength(100)] public string CustomerName { get; set; } = string.Empty;
    }

    public sealed class UpdateOrderItemRequest
    {
        [Range(0.0000001, double.MaxValue)] public double Quantity { get; set; }
        [Range(typeof(decimal), "0.01", "999999999999.99")] public decimal UnitPrice { get; set; }
        [Range(typeof(decimal), "0", "999999999999.99")] public decimal Discount { get; set; }
    }
}