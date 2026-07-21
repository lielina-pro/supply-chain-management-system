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
    public Task<ServiceResult<PurchaseOrderDto>> CreateAsync(
        CreatePurchaseOrderRequest req,
        CancellationToken ct = default) =>
        CreateAsync(req, createdByUserId: 1, ct);

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
                " A purchase order can only be created from an approved purchase request.");

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

        // This implementation creates a PO already in "Sent" (Week 4 demo flow).
        var sentStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, "Sent", ct)
            ?? throw new BusinessRuleException("PurchaseOrder status 'Sent' is not seeded.");

        var now = DateTime.UtcNow;
        var items = (req.Items.Count > 0
                ? req.Items.Select(i => new PurchaseOrderItem
                {
                    ProductId       = i.ProductId,
                    QuantityOrdered = i.Quantity,
                    UnitPrice       = i.UnitPrice
                })
                : request.Items.Select(i => new PurchaseOrderItem
                {
                    ProductId       = i.ProductId,
                    QuantityOrdered = i.QuantityRequested,
                    UnitPrice       = i.Product.UnitPrice
                }))
            .ToList();

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

    public async Task<IReadOnlyList<PurchaseOrderListItemDto>> GetAllAsync(CancellationToken ct = default)
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

    public async Task<ServiceResult<bool>> SendToSupplierAsync(int id, CancellationToken ct = default)
    {
        var po = await _uow.PurchaseOrders.GetWithDetailsAsync(id, ct);
        if (po is null)
            return ServiceResult<bool>.Failure("Purchase order not found.");

        if (po.Status.StatusName == "Sent")
            return ServiceResult<bool>.Success(true);

        if (po.Status.StatusName != "Draft")
            return ServiceResult<bool>.Failure($"Only a Draft purchase order can be sent (currently {po.Status.StatusName}).");

        var sentStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, "Sent", ct)
            ?? throw new BusinessRuleException("PurchaseOrder status 'Sent' is not seeded.");

        await _uow.StatusHistory.AddAsync(new StatusHistory
        {
            EntityType      = EntityType,
            EntityId        = po.Id,
            FromStatusId    = po.StatusId,
            ToStatusId      = sentStatus.Id,
            ChangedByUserId = 1,
            ChangedAt       = DateTime.UtcNow,
            Notes           = "Purchase order sent to supplier"
        }, ct);

        po.StatusId = sentStatus.Id;
        _uow.PurchaseOrders.Update(po);
        await _uow.SaveChangesAsync(ct);
        return ServiceResult<bool>.Success(true);
    }

    // ── FR-03.5 record delivery ───────────────────────────────────────────────
    public async Task<ServiceResult<bool>> RecordDeliveryAsync(
        int id,
        RecordDeliveryRequest req,
        int performedByUserId,
        CancellationToken ct = default)
    {
        var po = await _uow.PurchaseOrders.GetWithDetailsAsync(id, ct);
        if (po is null)
            return ServiceResult<bool>.Failure("Purchase order not found.");

        if (po.Status.StatusName is "Delivered" or "Cancelled")
            return ServiceResult<bool>.Failure($"Cannot record delivery for a {po.Status.StatusName} purchase order.");

        var warehouse = await _uow.Warehouses.GetByIdAsync(req.WarehouseId, ct);
        if (warehouse is null || warehouse.IsArchived || !warehouse.IsActive)
            return ServiceResult<bool>.Failure("Warehouse not found or inactive.");

        if (req.Lines.Count == 0)
            return ServiceResult<bool>.Failure("At least one delivery line is required.");

        var now = DateTime.UtcNow;

        foreach (var line in req.Lines)
        {
            if (line.QuantityReceived <= 0)
                return ServiceResult<bool>.Failure("Quantity received must be greater than zero.");

            var item = po.Items.FirstOrDefault(i => i.Id == line.PurchaseOrderItemId);
            if (item is null)
                return ServiceResult<bool>.Failure($"Purchase order item {line.PurchaseOrderItemId} does not belong to this order.");

            var remaining = item.QuantityOrdered - item.QuantityReceived;
            if (line.QuantityReceived > remaining)
                return ServiceResult<bool>.Failure($"Cannot receive {line.QuantityReceived} units for product {item.ProductId}; only {remaining} remain.");

            item.QuantityReceived += line.QuantityReceived;

            var stock = await _uow.InventoryStocks.GetByProductAndWarehouseAsync(item.ProductId, req.WarehouseId, ct);
            if (stock is null)
            {
                stock = new InventoryStock
                {
                    ProductId        = item.ProductId,
                    WarehouseId      = req.WarehouseId,
                    QuantityOnHand   = 0,
                    QuantityReserved = 0,
                    UpdatedAt        = now
                };
                await _uow.InventoryStocks.AddAsync(stock, ct);
            }

            stock.QuantityOnHand += line.QuantityReceived;
            stock.UpdatedAt = now;
            _uow.InventoryStocks.Update(stock);

            // BR-04: permanent stock ledger entry
            await _uow.StockTransactions.AddAsync(new StockTransaction
            {
                ProductId         = item.ProductId,
                WarehouseId       = req.WarehouseId,
                TransactionType   = "receipt",
                Quantity          = line.QuantityReceived,
                ReferenceType     = "PurchaseOrder",
                ReferenceId       = po.Id,
                PerformedByUserId = performedByUserId,
                CreatedAt         = now
            }, ct);
        }

        // Update PO status based on completion
        var allFull = po.Items.All(i => i.QuantityReceived >= i.QuantityOrdered);
        var anyReceived = po.Items.Any(i => i.QuantityReceived > 0);
        var targetStatusName = allFull ? "Delivered" : (anyReceived ? "PartiallyDelivered" : po.Status.StatusName);

        if (targetStatusName != po.Status.StatusName)
        {
            var targetStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, targetStatusName, ct)
                ?? throw new BusinessRuleException($"PurchaseOrder status '{targetStatusName}' is not seeded.");

            await _uow.StatusHistory.AddAsync(new StatusHistory
            {
                EntityType      = EntityType,
                EntityId        = po.Id,
                FromStatusId    = po.StatusId,
                ToStatusId      = targetStatus.Id,
                ChangedByUserId = performedByUserId,
                ChangedAt       = now,
                Notes           = "Updated from delivery recording"
            }, ct);

            po.StatusId = targetStatus.Id;
            _uow.PurchaseOrders.Update(po);
        }

        await _uow.SaveChangesAsync(ct);
        return ServiceResult<bool>.Success(true);
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
            Id               = i.Id,
            ProductId        = i.ProductId,
            ProductName      = i.Product.Name,
            SKU              = i.Product.SKU,
            QuantityOrdered  = i.QuantityOrdered,
            QuantityReceived = i.QuantityReceived,
            UnitPrice        = i.UnitPrice
        }).ToList()
    };
}
