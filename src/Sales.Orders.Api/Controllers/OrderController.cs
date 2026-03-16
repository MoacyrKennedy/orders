using Microsoft.AspNetCore.Mvc;

namespace Sales.Orders.Api.Controllers;

[ApiController]
[Route("order/api")]
public class OrderController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Ok");
    }
}