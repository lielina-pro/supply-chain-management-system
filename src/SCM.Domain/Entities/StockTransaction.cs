namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table StockTransactions. Deliberately has NO IsArchived column -
/// this is a permanent, immutable ledger (BR-04). Mistakes are corrected by
/// inserting a new reversing transaction, never by hiding or deleting a row.
/// </summary>
public class StockTransaction
{
    public int       Id          { get; set; }
    public int       ProductId   { get; set; }
    public Product   Product     { get; set; } = default!;
    public int       WarehouseId { get; set; }
    public Warehouse Warehouse   { get; set; } = default!;

    public string TransactionType { get; set; } = default!; // receipt | issue | transfer | adjustment | reservation_release
    public int    Quantity        { get; set; } // signed: negative = decrease
    public string? ReferenceType  { get; set; } // PurchaseOrder | CustomerOrder | Manual
    public int?    ReferenceId    { get; set; }

    public int     PerformedByUserId { get; set; }
    public string? Reason            { get; set; }
    public DateTime CreatedAt        { get; set; }
}
