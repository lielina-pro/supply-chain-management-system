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
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? "0");

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
    public async Task<IActionResult> GetAll()
    {
        var list = await _svc.GetAllAsync();
        return Ok(list);
    }

    // FR-03.4 — get single PO with items
    [HttpGet("{id}")]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }
}