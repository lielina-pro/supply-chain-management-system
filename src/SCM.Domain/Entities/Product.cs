using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary> Maps to ER table Products. </summary>
public class Product : BaseEntity, IArchivable
{
    public string   SKU               { get; set; } = default!;
    public string   Name              { get; set; } = default!;
    public string?  Description       { get; set; }
    public string?  Category          { get; set; }
    public decimal? UnitPrice         { get; set; }
    public int?     ReorderThreshold  { get; set; }
    public bool     IsActive          { get; set; } = true;

    public bool      IsArchived       { get; set; }
    public DateTime? ArchivedAt       { get; set; }
    public int?      ArchivedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseRequestItem> PurchaseRequestItems { get; set; } = new();
    public List<PurchaseOrderItem>   PurchaseOrderItems    { get; set; } = new();
}
