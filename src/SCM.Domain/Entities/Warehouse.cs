using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table Warehouses (minimal fields needed to support FR-03.5
/// delivery recording - Racks/Bins are not modeled yet, that's the
/// Warehouse module's own future week).
/// </summary>
public class Warehouse : BaseEntity, IArchivable
{
    public string   Name     { get; set; } = default!;
    public string?  Location { get; set; }
    public int?     Capacity { get; set; }
    public bool     IsActive { get; set; } = true;

    public bool      IsArchived       { get; set; }
    public DateTime? ArchivedAt       { get; set; }
    public int?      ArchivedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
