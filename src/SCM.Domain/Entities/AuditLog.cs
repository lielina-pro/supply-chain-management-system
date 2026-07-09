using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary> Maps to ER table AuditLogs (BR-10: all critical transactions auditable). </summary>
public class AuditLog : BaseEntity
{
    public int      UserId     { get; set; }
    public string   Action     { get; set; } = default!;
    public string?  EntityName { get; set; }
    public int?      EntityId  { get; set; }
    public string?  Details    { get; set; }
    public string?  IPAddress  { get; set; }
    public string?  UserAgent  { get; set; }
    public bool     IsArchived { get; set; }
    public DateTime? ArchivedAt{ get; set; }
    public DateTime Timestamp  { get; set; }
}
