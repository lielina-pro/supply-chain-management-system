using System.ComponentModel.DataAnnotations;

namespace SCM.Web.ViewModels;

public class PurchaseOrderListItemViewModel
{
    public int      Id                  { get; set; }
    public string   SupplierCompanyName { get; set; } = default!;
    public string   StatusName          { get; set; } = default!;
    public DateTime OrderDate           { get; set; }
    public decimal? TotalAmount         { get; set; }
}

public class PurchaseOrderItemViewModel
{
    public int      Id               { get; set; }
    public string   ProductName      { get; set; } = default!;
    public string   SKU              { get; set; } = default!;
    public int      QuantityOrdered  { get; set; }
    public int      QuantityReceived { get; set; }
    public decimal? UnitPrice        { get; set; }
}

public class PurchaseOrderDetailViewModel
{
    public int      Id                    { get; set; }
    public int      PurchaseRequestId     { get; set; }
    public string   SupplierCompanyName   { get; set; } = default!;
    public string   StatusName            { get; set; } = default!;
    public DateTime OrderDate             { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal? TotalAmount           { get; set; }
    public List<PurchaseOrderItemViewModel> Items { get; set; } = new();
}

/// <summary>
/// FR-03.3 form. Purchase Request / Supplier are entered by ID for now -
/// swap for real dropdowns once Bethel's purchase-request list endpoint exists.
/// </summary>
public class CreatePurchaseOrderViewModel
{
    [Required, Display(Name = "Purchase request ID (must be Approved)")]
    public int PurchaseRequestId { get; set; }

    [Required, Display(Name = "Supplier ID (must be Active)")]
    public int SupplierId { get; set; }

    [Display(Name = "Expected delivery date")]
    [DataType(DataType.Date)]
    public DateTime? ExpectedDeliveryDate { get; set; }
}

/// <summary>FR-03.5 - one line of a delivery being recorded, pre-filled from the PO's items.</summary>
public class DeliveryLineViewModel
{
    public int    PurchaseOrderItemId { get; set; }
    public string ProductName         { get; set; } = default!;
    public int    Remaining           { get; set; }
    public int    QuantityReceived    { get; set; }
}

public class RecordDeliveryViewModel
{
    public int PurchaseOrderId { get; set; }

    [Required, Display(Name = "Receiving warehouse ID")]
    public int WarehouseId { get; set; } = 1; // seeded "Main Warehouse" is Id 1 in a fresh database

    public List<DeliveryLineViewModel> Lines { get; set; } = new();
}
