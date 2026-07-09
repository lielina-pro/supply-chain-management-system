using SCM.Application.Common.Models;

namespace SCM.Application.Inventory.Interfaces;

// Placeholder so InventoryController compiles. Not implemented/registered yet -
// this is the Week 5 module (FR-04).
public interface IInventoryService
{
    Task<object>              GetAllStockLevelsAsync(CancellationToken ct = default);
    Task<ServiceResult<object>> RecordReceiptAsync(object req, CancellationToken ct = default);
    Task<ServiceResult<object>> RecordAdjustmentAsync(object req, CancellationToken ct = default);
}
