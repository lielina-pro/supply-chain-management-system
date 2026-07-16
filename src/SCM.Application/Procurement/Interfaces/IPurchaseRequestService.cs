using SCM.Application.Common.Models;
using SCM.Application.Procurement.DTOs;

namespace SCM.Application.Procurement.Interfaces;

public interface IPurchaseRequestService
{
    // FR-03.1 — create a purchase request
    Task<ServiceResult<PurchaseRequestDto>> CreateAsync(
        CreatePurchaseRequestRequest req,
        int requestedByUserId,
        CancellationToken ct = default);

    // FR-03.2 — record an approval step
    Task<ServiceResult<PurchaseRequestDto>> RecordApprovalAsync(
        int requestId,
        string approverRole,
        RecordApprovalRequest req,
        int approverUserId,
        CancellationToken ct = default);

    // FR-03.4 — read
    Task<PurchaseRequestDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PurchaseRequestListItemDto>> GetAllAsync(CancellationToken ct = default);
}