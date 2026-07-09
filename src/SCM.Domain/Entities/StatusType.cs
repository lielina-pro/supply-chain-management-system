using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table StatusTypes. One shared table for every entity that has a
/// status (Supplier, CustomerOrder, PurchaseOrder, ...), filtered by EntityType.
/// A plain FK can't restrict a referencing table to only its own EntityType rows -
/// that scoping is enforced in the Application service layer here (the ER notes
/// this would be a DB trigger in the full SQL Server schema).
/// </summary>
public class StatusType : BaseEntity
{
    public string   EntityType { get; set; } = default!;
    public string   StatusName { get; set; } = default!;
    public bool     IsArchived { get; set; }
    public int      CreatedBy  { get; set; }
    public DateTime CreatedAt  { get; set; }
}
