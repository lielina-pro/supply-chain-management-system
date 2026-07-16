namespace SCM.Application.Procurement.DTOs;

// ── Request DTOs (what the API receives) ─────────────────────────────────────

public class CreatePurchaseRequestRequest
{
    public List<PurchaseRequestItemInput> Items { get; set; } = new();
    public string? Notes { get; set; }
}

public class PurchaseRequestItemInput
{
    public int ProductId         { get; set; }
    public int QuantityRequested { get; set; }
}

public class RecordApprovalRequest
{
    public string  Decision { get; set; } = default!; // Validated | Approved | Declined | AdjustmentRequested
    public string? Comments { get; set; }
}

// ── Response DTOs (what the API returns) ─────────────────────────────────────

public class PurchaseRequestDto
{
    public int      Id            { get; set; }
    public string   RequestedBy   { get; set; } = default!;
    public string   Status        { get; set; } = default!;
    public decimal? EstimatedCost { get; set; }
    public string?  Notes         { get; set; }
    public DateTime CreatedAt     { get; set; }
    public int?     ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt   { get; set; }
    public List<PurchaseRequestItemDto>     Items     { get; set; } = new();
    public List<PurchaseRequestApprovalDto> Approvals { get; set; } = new();
}

public class PurchaseRequestItemDto
{
    public int    ProductId   { get; set; }
    public string ProductName { get; set; } = default!;
    public int    QuantityRequested { get; set; }
}

public class PurchaseRequestApprovalDto
{
    public int      StepOrder    { get; set; }
    public string   ApproverRole { get; set; } = default!;
    public string?  ApproverName { get; set; }
    public string   Decision     { get; set; } = default!;
    public string?  Comments     { get; set; }
    public DateTime DecidedAt    { get; set; }
}

public class PurchaseRequestListItemDto
{
    public int      Id            { get; set; }
    public string   RequestedBy   { get; set; } = default!;
    public string   Status        { get; set; } = default!;
    public decimal? EstimatedCost { get; set; }
    public int      ItemCount     { get; set; }
    public DateTime CreatedAt     { get; set; }
}
// ── Purchase Order DTOs ───────────────────────────────────────────────────────

public class CreatePurchaseOrderRequest
{
    public int                         PurchaseRequestId    { get; set; }
    public int                         SupplierId           { get; set; }
    public DateTime?                   ExpectedDeliveryDate { get; set; }
    public List<PurchaseOrderItemInput> Items               { get; set; } = new();
}

public class PurchaseOrderItemInput
{
    public int     ProductId { get; set; }
    public int     Quantity  { get; set; }
    public decimal UnitPrice { get; set; }
}

public class RecordDeliveryRequest
{
    public int WarehouseId { get; set; }
    public List<RecordDeliveryLineInput> Lines { get; set; } = new();
}

public class RecordDeliveryLineInput
{
    public int PurchaseOrderItemId { get; set; }
    public int QuantityReceived { get; set; }
}

// Backward-compatible alias for earlier MVC code.
public class DeliveryLineInput : RecordDeliveryLineInput
{
}

public class PurchaseOrderDto
{
    public int       Id                   { get; set; }
    public int       PurchaseRequestId    { get; set; }
    public string    SupplierName         { get; set; } = default!;
    public string    Status               { get; set; } = default!;
    public DateTime  OrderDate            { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal?  TotalAmount          { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new();

    // Backward-compatible aliases for earlier MVC code.
    public string SupplierCompanyName => SupplierName;
    public string StatusName => Status;
}

public class PurchaseOrderItemDto
{
    public int      Id               { get; set; }
    public int      ProductId        { get; set; }
    public string   ProductName      { get; set; } = default!;
    public string   SKU              { get; set; } = default!;
    public int      QuantityOrdered  { get; set; }
    public int      QuantityReceived { get; set; }
    public decimal? UnitPrice        { get; set; }
}

public class PurchaseOrderListItemDto
{
    public int      Id                { get; set; }
    public string   SupplierName      { get; set; } = default!;
    public string   Status            { get; set; } = default!;
    public decimal? TotalAmount       { get; set; }
    public DateTime OrderDate         { get; set; }
    public int      ItemCount         { get; set; }

    // Backward-compatible aliases for earlier MVC code.
    public string SupplierCompanyName => SupplierName;
    public string StatusName => Status;
}