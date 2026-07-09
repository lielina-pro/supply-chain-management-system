using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.Application.Suppliers.DTOs;
using SCM.Application.Suppliers.Interfaces;

namespace SCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _svc;
    public SuppliersController(ISupplierService svc) => _svc = svc;

    /// <summary>FR-02.1 - Supplier self-registration (creates linked User + Supplier).</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateSupplierRequest req)
    {
        var result = await _svc.RegisterAsync(req);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>FR-02.5 - Supplier list.</summary>
    [HttpGet]
    [Authorize] // Admin: full, ProcurementManager/FinanceAnalyst: read, per the RBAC matrix
    public async Task<IActionResult> GetAll()
    {
        var list = await _svc.GetAllAsync();
        return Ok(list);
    }

    /// <summary>FR-02.2 - Supplier profile.</summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>FR-02.2 - Update supplier profile (Supplier: own record; Admin: any).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "SupplierPortal")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateSupplierProfileRequest req)
    {
        var result = await _svc.UpdateProfileAsync(id, req);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>FR-02.4 - Activate / deactivate supplier (BR-05).</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "ProcurementAccess")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateSupplierStatusRequest req)
    {
        // TODO: once auth is wired, resolve the acting user id from the JWT claims
        // instead of passing null (ChangedByUserId is nullable to allow automated jobs too).
        var result = await _svc.UpdateStatusAsync(id, req, actingUserId: null);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
