using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table InventoryStock. BinId is left as a plain nullable int
/// (soft link) since Bins isn't modeled yet - Warehouse module, later week.
/// BR-01 (quantity never negative) is enforced in the service layer, not here.
/// </summary>
public class InventoryStock : BaseEntity
{
    public int       ProductId   { get; set; }
    public Product   Product     { get; set; } = default!;
    public int       WarehouseId { get; set; }
    public Warehouse Warehouse   { get; set; } = default!;
    public int?      BinId       { get; set; }

    public int QuantityOnHand   { get; set; } // BR-01: never < 0
    public int QuantityReserved { get; set; } // BR-01: never < 0

    public bool      IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime? UpdatedAt  { get; set; }
}
