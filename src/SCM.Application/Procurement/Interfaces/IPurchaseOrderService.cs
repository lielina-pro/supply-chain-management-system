using SCM.Application.Common.Models;
using SCM.Application.Procurement.DTOs;

namespace SCM.Application.Procurement.Interfaces;

public interface IPurchaseOrderService
{
    // FR-03.3 — BR-02 and BR-05 enforced here
    Task<ServiceResult<PurchaseOrderDto>> CreateAsync(
        CreatePurchaseOrderRequest req,
        CancellationToken ct = default);

    Task<ServiceResult<PurchaseOrderDto>> CreateAsync(
        CreatePurchaseOrderRequest req,
        int createdByUserId,
        CancellationToken ct = default);

    // FR-03.4
    Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PurchaseOrderListItemDto>> GetAllAsync(CancellationToken ct = default);

    // Compatibility for earlier MVC flow
    Task<ServiceResult<bool>> SendToSupplierAsync(int id, CancellationToken ct = default);

    // FR-03.5
    Task<ServiceResult<bool>> RecordDeliveryAsync(
        int id,
        RecordDeliveryRequest req,
        int performedByUserId,
        CancellationToken ct = default);
}
