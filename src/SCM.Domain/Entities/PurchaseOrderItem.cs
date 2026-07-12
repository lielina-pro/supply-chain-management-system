namespace SCM.Domain.Entities;

/// <summary> Maps to ER table PurchaseOrderItems. QuantityReceived is updated by FR-03.5. </summary>
public class PurchaseOrderItem
{
    public int Id { get; set; }

    public int           PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder   { get; set; } = default!;

    public int     ProductId { get; set; }
    public Product Product   { get; set; } = default!;

    public int      QuantityOrdered  { get; set; }
    public int      QuantityReceived { get; set; } = 0;
    public decimal? UnitPrice        { get; set; }

    public bool      IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}
