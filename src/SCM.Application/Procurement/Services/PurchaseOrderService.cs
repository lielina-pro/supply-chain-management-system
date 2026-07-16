using SCM.Application.Common.Exceptions;
using SCM.Application.Common.Models;
using SCM.Application.Procurement.DTOs;
using SCM.Application.Procurement.Interfaces;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces;

namespace SCM.Application.Procurement.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private const string EntityType = "PurchaseOrder";
    private readonly IUnitOfWork _uow;

    public PurchaseOrderService(IUnitOfWork uow) => _uow = uow;

    // ── FR-03.3 Create PO ─────────────────────────────────────────────────────
    public async Task<ServiceResult<PurchaseOrderDto>> CreateAsync(
        CreatePurchaseOrderRequest req,
        int createdByUserId,
        CancellationToken ct = default)
    {
        // BR-02: only approved requests may generate POs
        var request = await _uow.PurchaseRequests.GetWithDetailsAsync(req.PurchaseRequestId, ct);
        if (request is null)
            return ServiceResult<PurchaseOrderDto>.Failure("Purchase request not found.");

        if (request.Status.StatusName != "Approved")
            return ServiceResult<PurchaseOrderDto>.Failure(
                "BR-02: A purchase order can only be created from an approved purchase request.");

        if (request.PurchaseOrders.Any())
            return ServiceResult<PurchaseOrderDto>.Failure(
                "A purchase order already exists for this request.");

        // BR-05: only active suppliers may receive POs
        var supplier = await _uow.Suppliers.GetWithDetailsAsync(req.SupplierId, ct);
        if (supplier is null)
            return ServiceResult<PurchaseOrderDto>.Failure("Supplier not found.");

        if (supplier.Status.StatusName != "Active")
            return ServiceResult<PurchaseOrderDto>.Failure(
                "BR-05: Purchase orders can only be sent to active suppliers.");

        var sentStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, "Sent", ct)
            ?? throw new BusinessRuleException("PurchaseOrder status 'Sent' is not seeded.");

        var now   = DateTime.UtcNow;
        var items = req.Items.Select(i => new PurchaseOrderItem
        {
            ProductId       = i.ProductId,
            QuantityOrdered = i.Quantity,
            UnitPrice       = i.UnitPrice
        }).ToList();

        var totalAmount = items.Sum(i => (i.UnitPrice ?? 0) * i.QuantityOrdered);

        var po = new PurchaseOrder
        {
            PurchaseRequestId    = req.PurchaseRequestId,
            SupplierId           = req.SupplierId,
            StatusId             = sentStatus.Id,
            OrderDate            = now,
            ExpectedDeliveryDate = req.ExpectedDeliveryDate,
            TotalAmount          = totalAmount
        };
        po.Items.AddRange(items);

        await _uow.PurchaseOrders.AddAsync(po, ct);

        await _uow.StatusHistory.AddAsync(new StatusHistory
        {
            EntityType      = EntityType,
            EntityId        = po.Id,
            FromStatusId    = null,
            ToStatusId      = sentStatus.Id,
            ChangedByUserId = createdByUserId,
            ChangedAt       = now,
            Notes           = "Purchase order created and sent to supplier"
        }, ct);

        await _uow.AuditLogs.AddAsync(new AuditLog
        {
            UserId     = createdByUserId,
            Action     = "CreatePurchaseOrder",
            EntityName = "PurchaseOrder",
            EntityId   = po.Id,
            Details    = $"PO for supplier {req.SupplierId}, total {totalAmount}. BR-02 passed.",
            Timestamp  = now
        }, ct);

        await _uow.SaveChangesAsync(ct);

        var saved = await _uow.PurchaseOrders.GetWithDetailsAsync(po.Id, ct);
        return ServiceResult<PurchaseOrderDto>.Success(MapToDto(saved!));
    }

    // ── FR-03.4 reads ─────────────────────────────────────────────────────────
    public async Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var po = await _uow.PurchaseOrders.GetWithDetailsAsync(id, ct);
        return po is null ? null : MapToDto(po);
    }

    public async Task<IReadOnlyList<PurchaseOrderListItemDto>> GetAllAsync(
        CancellationToken ct = default)
    {
        var list = await _uow.PurchaseOrders.GetAllWithDetailsAsync(ct);
        return list.Select(po => new PurchaseOrderListItemDto
        {
            Id           = po.Id,
            SupplierName = po.Supplier.CompanyName,
            Status       = po.Status.StatusName,
            TotalAmount  = po.TotalAmount,
            OrderDate    = po.OrderDate,
            ItemCount    = po.Items.Count
        }).ToList();
    }

    // ── Mapper ────────────────────────────────────────────────────────────────
    private static PurchaseOrderDto MapToDto(PurchaseOrder po) => new()
    {
        Id                   = po.Id,
        PurchaseRequestId    = po.PurchaseRequestId,
        SupplierName         = po.Supplier.CompanyName,
        Status               = po.Status.StatusName,
        OrderDate            = po.OrderDate,
        ExpectedDeliveryDate = po.ExpectedDeliveryDate,
        TotalAmount          = po.TotalAmount,
        Items = po.Items.Select(i => new PurchaseOrderItemDto
        {
            ProductId        = i.ProductId,
            ProductName      = i.Product.Name,
            QuantityOrdered  = i.QuantityOrdered,
            QuantityReceived = i.QuantityReceived,
            UnitPrice        = i.UnitPrice
        }).ToList()
    };
}