using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table PurchaseOrders.
/// BR-02: PurchaseRequestId must reference a fully-approved request - enforced
/// in the service layer (PurchaseOrderService), not here.
/// BR-05: SupplierId must be an active supplier - also service-layer enforced.
/// </summary>
public class PurchaseOrder : BaseEntity, IArchivable
{
    public int             PurchaseRequestId { get; set; }
    public PurchaseRequest PurchaseRequest   { get; set; } = default!;

    public int      SupplierId { get; set; }
    public Supplier Supplier   { get; set; } = default!;

    public int         StatusId { get; set; } // FK -> StatusTypes (EntityType=PurchaseOrder)
    public StatusType  Status   { get; set; } = default!;

    public DateTime  OrderDate             { get; set; }
    public DateTime? ExpectedDeliveryDate  { get; set; }
    public decimal?  TotalAmount           { get; set; }

    public bool      IsArchived       { get; set; }
    public DateTime? ArchivedAt       { get; set; }
    public int?      ArchivedByUserId { get; set; }

    public List<PurchaseOrderItem> Items { get; set; } = new();
}
