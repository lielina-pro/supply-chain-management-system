using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.Application.Orders.Interfaces;

namespace SCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _svc;
    public OrdersController(IOrderService svc) => _svc = svc;

    //<summary>FR-06.1 – Customer places order</summary>
    [HttpPost]
    [Authorize(Policy = "CustomerPortal")]
    public async Task<IActionResult> PlaceOrder([FromBody] object req)
    {
        // FR-06.2: inventory check + FR-06.3: reservation on approval
        var result = await _svc.PlaceOrderAsync(req);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    //<summary>FR-06.4 / FR-06.6 – Track order status (real-time)</summary>
    [HttpGet("{id:int}/status")]
    public async Task<IActionResult> TrackOrder(int id)
    {
        var status = await _svc.GetOrderStatusAsync(id);
        return status is null ? NotFound() : Ok(status);
    }

    //<summary>FR-06.5 – Cancel order (BR-11: only pre-shipment)</summary>
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _svc.CancelOrderAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
