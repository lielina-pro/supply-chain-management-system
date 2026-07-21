using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.Application.Procurement.DTOs;
using SCM.Application.Procurement.Interfaces;
using System.Security.Claims;

namespace SCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IPurchaseRequestService _svc;
    public PurchaseRequestsController(IPurchaseRequestService svc) => _svc = svc;

    // Reads the user ID from the JWT sub claim (set by your Week 3 JwtTokenService)
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? "0");

    // Reads the first role claim from the JWT
    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    // FR-03.1 — Procurement Manager creates a request
    [HttpPost]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseRequestRequest req)
    {
        var result = await _svc.CreateAsync(req, CurrentUserId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // FR-03.4 — list all requests
    [HttpGet]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _svc.GetAllAsync();
        return Ok(list);
    }

    // FR-03.4 — get single request with full approval history
    [HttpGet("{id}")]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    // FR-03.2 — Warehouse Manager validates (step 1)
    [HttpPatch("{id}/validate")]
    [Authorize(Policy = "WarehouseAccess")]
    public async Task<IActionResult> Validate(int id, [FromBody] RecordApprovalRequest req)
    {
        // Force decision to valid options for this step
        if (req.Decision is not ("Validated" or "Declined" or "AdjustmentRequested"))
            return BadRequest("Decision must be Validated");

        var result = await _svc.RecordApprovalAsync(id, "WarehouseManager", req, CurrentUserId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // FR-03.2 — Finance Analyst approves budget (step 2)
    [HttpPatch("{id}/approve")]
    [Authorize(Policy = "FinanceAccess")]
    public async Task<IActionResult> Approve(int id, [FromBody] RecordApprovalRequest req)
    {
        if (req.Decision is not ("Approved" or "Declined" or "AdjustmentRequested"))
            return BadRequest("Decision must be Validated");

        var result = await _svc.RecordApprovalAsync(id, "FinanceAnalyst", req, CurrentUserId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}