namespace SCM.Domain.Entities;

/// <summary> Maps to ER table UserRoles (BR-09: every user must belong to at least one role). </summary>
public class UserRole
{
    public int      UserId     { get; set; }
    public User      User      { get; set; } = default!;
    public int      RoleId     { get; set; }
    public Role      Role      { get; set; } = default!;
    public DateTime AssignedAt { get; set; }
    public bool      IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}
