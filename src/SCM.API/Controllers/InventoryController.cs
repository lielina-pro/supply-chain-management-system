using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCM.Application.Inventory.Interfaces;

namespace SCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _svc;
    public InventoryController(IInventoryService svc) => _svc = svc;

    /// <summary>FR-04.5 – Real-time stock levels</summary>
    [HttpGet]
    [Authorize(Policy = "WarehouseAccess")]
    public async Task<IActionResult> GetStockLevels()
    {
        var levels = await _svc.GetAllStockLevelsAsync();
        return Ok(levels);
    }

    /// <summary>FR-04.1 – Record stock receipt</summary>
    [HttpPost("receipts")]
    [Authorize(Policy = "WarehouseAccess")]
    public async Task<IActionResult> Receipt([FromBody] object req)
    {
        // BR-01: inventory qty never goes negative (enforced in service)
        var result = await _svc.RecordReceiptAsync(req);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    /// <summary>FR-04.4 – Stock adjustment (BR-04: always logged)</summary>
    [HttpPost("adjustments")]
    [Authorize(Policy = "WarehouseAccess")]
    public async Task<IActionResult> Adjust([FromBody] object req)
    {
        var result = await _svc.RecordAdjustmentAsync(req);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
