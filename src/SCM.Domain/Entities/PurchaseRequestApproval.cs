namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table PurchaseRequestApprovals. One row per step in the
/// approval chain (WarehouseManager validates -> FinanceAnalyst approves
/// budget -> ProcurementManager sends to supplier). Multiple rows per
/// request = negotiation history.
/// </summary>
public class PurchaseRequestApproval
{
    public int Id { get; set; }

    public int             PurchaseRequestId { get; set; }
    public PurchaseRequest PurchaseRequest   { get; set; } = default!;

    public int    StepOrder    { get; set; }
    public string ApproverRole { get; set; } = default!; // WarehouseManager | FinanceAnalyst | ProcurementManager
    public int?   ApproverUserId { get; set; }
    public User?  ApproverUser   { get; set; }

    public string  Decision { get; set; } = default!; // Validated | Approved | Declined | AdjustmentRequested
    public string? Comments { get; set; }

    public bool      IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime  DecidedAt  { get; set; }
}
