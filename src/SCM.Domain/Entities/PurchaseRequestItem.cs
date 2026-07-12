namespace SCM.Domain.Entities;

/// <summary> Maps to ER table PurchaseRequestItems. </summary>
public class PurchaseRequestItem
{
    public int Id { get; set; }

    public int             PurchaseRequestId { get; set; }
    public PurchaseRequest PurchaseRequest   { get; set; } = default!;

    public int     ProductId { get; set; }
    public Product Product   { get; set; } = default!;

    public int QuantityRequested { get; set; }

    public bool      IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}
