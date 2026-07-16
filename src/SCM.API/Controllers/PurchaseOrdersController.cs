using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.Application.Procurement.DTOs;
using SCM.Application.Procurement.Interfaces;
using System.Security.Claims;

namespace SCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _svc;
    public PurchaseOrdersController(IPurchaseOrderService svc) => _svc = svc;

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id
            : 0;

    // FR-03.3 — create PO (BR-02 + BR-05 enforced in service)
    [HttpPost]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest req)
    {
        var result = await _svc.CreateAsync(req, CurrentUserId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // FR-03.4 — list all POs
    [HttpGet]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    // FR-03.4 — get single PO with items
    [HttpGet("{id:int}")]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    // FR-03.5 — record units received per line (updates InventoryStock + logs StockTransaction - BR-04)
    [HttpPatch("{id:int}/receive")]
    [Authorize(Policy = "WarehouseAccess")]
    public async Task<IActionResult> RecordDelivery(int id, [FromBody] RecordDeliveryRequest req)
    {
        if (CurrentUserId == 0)
            return Unauthorized("Could not resolve the acting user from the token.");

        var result = await _svc.RecordDeliveryAsync(id, req, CurrentUserId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
