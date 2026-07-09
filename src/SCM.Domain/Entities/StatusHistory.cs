using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table StatusHistory. Full timeline per record - who changed the
/// status and when. EntityType/EntityId is a soft link (application-enforced),
/// not a real FK, so it can point at any status-bearing table.
/// </summary>
public class StatusHistory : BaseEntity
{
    public string   EntityType      { get; set; } = default!;
    public int      EntityId        { get; set; }
    public int?      FromStatusId   { get; set; } // null on first status ever set
    public int      ToStatusId      { get; set; }
    public int?      ChangedByUserId{ get; set; } // null if changed by an automated job
    public DateTime ChangedAt       { get; set; }
    public string?  Notes           { get; set; }
}
