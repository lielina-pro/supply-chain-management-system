using SCM.Application.Common.Exceptions;
using SCM.Application.Common.Models;
using SCM.Application.Procurement.DTOs;
using SCM.Application.Procurement.Interfaces;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces;

namespace SCM.Application.Procurement.Services;

public class PurchaseRequestService : IPurchaseRequestService
{
    private const string EntityType = "PurchaseRequest";

    // Roles that can act at each step — must match RoleNames constants exactly
    private const string WhmRole      = "WarehouseManager";
    private const string FinanceRole  = "FinanceAnalyst";

    private readonly IUnitOfWork _uow;
    public PurchaseRequestService(IUnitOfWork uow) => _uow = uow;

    // ── FR-03.1 ──────────────────────────────────────────────────────────────
    public async Task<ServiceResult<PurchaseRequestDto>> CreateAsync(
        CreatePurchaseRequestRequest req,
        int requestedByUserId,
        CancellationToken ct = default)
    {
        if (!req.Items.Any())
            return ServiceResult<PurchaseRequestDto>.Failure(
                "A purchase request must have at least one item.");

        // Resolve Pending status
        var pendingStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, "Pending", ct)
            ?? throw new BusinessRuleException(
                "StatusType 'Pending' for PurchaseRequest is not seeded.");

        var now = DateTime.UtcNow;

        // Calculate estimated cost + build items
        decimal estimatedCost = 0;
        var items = new List<PurchaseRequestItem>();
        foreach (var input in req.Items)
        {
            var product = await _uow.Products.GetByIdAsync(input.ProductId, ct);
            if (product is null)
                return ServiceResult<PurchaseRequestDto>.Failure(
                    $"Product with ID {input.ProductId} was not found.");

            estimatedCost += (product.UnitPrice ?? 0) * input.QuantityRequested;
            items.Add(new PurchaseRequestItem
            {
                ProductId         = input.ProductId,
                QuantityRequested = input.QuantityRequested
            });
        }

        var request = new PurchaseRequest
        {
            RequestedByUserId = requestedByUserId,
            StatusId          = pendingStatus.Id,
            EstimatedCost     = estimatedCost,
            Notes             = req.Notes,
            CreatedAt         = now
        };
        request.Items.AddRange(items);

        await _uow.PurchaseRequests.AddAsync(request, ct);
        await _uow.SaveChangesAsync(ct); // assigns Id

        // Initial status history row (FromStatusId = null — first status ever)
        await _uow.StatusHistory.AddAsync(new StatusHistory
        {
            EntityType      = EntityType,
            EntityId        = request.Id,
            FromStatusId    = null,
            ToStatusId      = pendingStatus.Id,
            ChangedByUserId = requestedByUserId,
            ChangedAt       = now,
            Notes           = "Purchase request created"
        }, ct);

        // Audit log (BR-10)
        await _uow.AuditLogs.AddAsync(new AuditLog
        {
            UserId     = requestedByUserId,
            Action     = "CreatePurchaseRequest",
            EntityName = "PurchaseRequest",
            EntityId   = request.Id,
            Details    = $"{items.Count} item(s), estimated cost {estimatedCost:F2}",
            Timestamp  = now
        }, ct);

        await _uow.SaveChangesAsync(ct);

        var saved = await _uow.PurchaseRequests.GetWithDetailsAsync(request.Id, ct);
        return ServiceResult<PurchaseRequestDto>.Success(MapToDto(saved!));
    }

    // ── FR-03.2 ──────────────────────────────────────────────────────────────
    public async Task<ServiceResult<PurchaseRequestDto>> RecordApprovalAsync(
        int requestId,
        string approverRole,
        RecordApprovalRequest req,
        int approverUserId,
        CancellationToken ct = default)
    {
        var request = await _uow.PurchaseRequests.GetWithDetailsAsync(requestId, ct);
        if (request is null)
            return ServiceResult<PurchaseRequestDto>.Failure("Purchase request not found.");

        // Cannot act on already-closed requests
        if (request.Status.StatusName is "Approved" or "Rejected")
            return ServiceResult<PurchaseRequestDto>.Failure(
                $"Cannot record an approval on a request that is already {request.Status.StatusName}.");

        var now        = DateTime.UtcNow;
        var nextStep   = request.Approvals.Count + 1;
        var prevStatus = request.StatusId;

        // ── Determine new request status based on decision ────────────────────
        string? newStatusName = null;

        if (req.Decision is "Declined")
        {
            newStatusName = "Rejected";
        }
        else if (req.Decision is "Validated" or "AdjustmentRequested")
        {
            // First touch — move from Pending to UnderReview
            if (request.Status.StatusName == "Pending")
                newStatusName = "UnderReview";
            // Otherwise leave status unchanged
        }
        else if (req.Decision is "Approved")
        {
            // Only FinanceAnalyst's Approved closes the chain
            if (approverRole == FinanceRole)
            {
                newStatusName = "Approved";
                request.ApprovedByUserId = approverUserId;
                request.ApprovedAt       = now;
            }
            else
            {
                // WarehouseManager "Approved" also moves to UnderReview
                if (request.Status.StatusName == "Pending")
                    newStatusName = "UnderReview";
            }
        }

        // ── Record the approval step ──────────────────────────────────────────
        var approvalRow = new PurchaseRequestApproval
        {
            PurchaseRequestId = requestId,
            StepOrder         = nextStep,
            ApproverRole      = approverRole,
            ApproverUserId    = approverUserId,
            Decision          = req.Decision,
            Comments          = req.Comments,
            DecidedAt         = now
        };
        // Add via EF navigation so it's tracked
        request.Approvals.Add(approvalRow);

        // ── Apply status change if needed ─────────────────────────────────────
        if (newStatusName is not null)
        {
            var newStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, newStatusName, ct)
                ?? throw new BusinessRuleException(
                    $"StatusType '{newStatusName}' for PurchaseRequest is not seeded.");

            request.StatusId = newStatus.Id;

            await _uow.StatusHistory.AddAsync(new StatusHistory
            {
                EntityType      = EntityType,
                EntityId        = requestId,
                FromStatusId    = prevStatus,
                ToStatusId      = newStatus.Id,
                ChangedByUserId = approverUserId,
                ChangedAt       = now,
                Notes           = $"Step {nextStep} — {approverRole}: {req.Decision}. {req.Comments}"
            }, ct);
        }

        _uow.PurchaseRequests.Update(request);

        // Audit log (BR-10)
        await _uow.AuditLogs.AddAsync(new AuditLog
        {
            UserId     = approverUserId,
            Action     = "RecordApproval",
            EntityName = "PurchaseRequest",
            EntityId   = requestId,
            Details    = $"Step {nextStep} | Role: {approverRole} | Decision: {req.Decision}",
            Timestamp  = now
        }, ct);

        await _uow.SaveChangesAsync(ct);

        var updated = await _uow.PurchaseRequests.GetWithDetailsAsync(requestId, ct);
        return ServiceResult<PurchaseRequestDto>.Success(MapToDto(updated!));
    }

    // ── FR-03.4 reads ─────────────────────────────────────────────────────────
    public async Task<PurchaseRequestDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var r = await _uow.PurchaseRequests.GetWithDetailsAsync(id, ct);
        return r is null ? null : MapToDto(r);
    }

    public async Task<IReadOnlyList<PurchaseRequestListItemDto>> GetAllAsync(
        CancellationToken ct = default)
    {
        var list = await _uow.PurchaseRequests.GetAllWithDetailsAsync(ct);
        return list.Select(r => new PurchaseRequestListItemDto
        {
            Id            = r.Id,
            RequestedBy   = r.RequestedByUser?.FullName ?? "System",
            Status        = r.Status.StatusName,
            EstimatedCost = r.EstimatedCost,
            ItemCount     = r.Items.Count,
            CreatedAt     = r.CreatedAt
        }).ToList();
    }

    // ── Mapper ────────────────────────────────────────────────────────────────
    private static PurchaseRequestDto MapToDto(PurchaseRequest r) => new()
    {
        Id               = r.Id,
        RequestedBy      = r.RequestedByUser?.FullName ?? "System",
        Status           = r.Status.StatusName,
        EstimatedCost    = r.EstimatedCost,
        Notes            = r.Notes,
        CreatedAt        = r.CreatedAt,
        ApprovedByUserId = r.ApprovedByUserId,
        ApprovedAt       = r.ApprovedAt,
        Items = r.Items.Select(i => new PurchaseRequestItemDto
        {
            ProductId        = i.ProductId,
            ProductName      = i.Product.Name,
            QuantityRequested = i.QuantityRequested
        }).ToList(),
        Approvals = r.Approvals
            .OrderBy(a => a.StepOrder)
            .Select(a => new PurchaseRequestApprovalDto
            {
                StepOrder    = a.StepOrder,
                ApproverRole = a.ApproverRole,
                ApproverName = a.ApproverUser?.FullName,
                Decision     = a.Decision,
                Comments     = a.Comments,
                DecidedAt    = a.DecidedAt
            }).ToList()
    };
}