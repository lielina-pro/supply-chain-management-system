using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table PurchaseRequests.
/// TriggeredByForecastId is kept as a plain nullable int (soft link) rather than
/// a real FK, because DemandForecasts isn't modeled yet (Forecasting module,
/// later week) - matches the ER's own soft-link approach elsewhere.
/// </summary>
public class PurchaseRequest : BaseEntity, IArchivable
{
    public int?     RequestedByUserId    { get; set; } // nullable if system-triggered
    public User?    RequestedByUser      { get; set; }
    public int?     TriggeredByForecastId{ get; set; } // soft link, no FK yet

    public int         StatusId { get; set; } // FK -> StatusTypes (EntityType=PurchaseRequest)
    public StatusType  Status   { get; set; } = default!;

    public decimal? EstimatedCost { get; set; }
    public string?  Notes         { get; set; }

    public int?      ApprovedByUserId { get; set; }
    public User?     ApprovedByUser   { get; set; }
    public DateTime? ApprovedAt       { get; set; }

    public bool      IsArchived       { get; set; }
    public DateTime? ArchivedAt       { get; set; }
    public int?      ArchivedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseRequestItem>     Items         { get; set; } = new();
    public List<PurchaseRequestApproval> Approvals     { get; set; } = new();
    public List<PurchaseOrder>           PurchaseOrders{ get; set; } = new();
}
