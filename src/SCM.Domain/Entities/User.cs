using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary> Maps to ER table Users. </summary>
public class User : BaseEntity, IArchivable
{
    public string    FullName            { get; set; } = default!;
    public string    Email               { get; set; } = default!;
    public string    PasswordHash        { get; set; } = default!;
    public string?   PhoneNumber         { get; set; }
    public bool      IsActive            { get; set; } = true;
    public bool      EmailConfirmed      { get; set; } = false;
    public DateTime? LastLoginAt         { get; set; }
    public int       FailedLoginAttempts { get; set; } = 0;
    public DateTime  CreatedAt           { get; set; }
    public DateTime? UpdatedAt           { get; set; }

    public bool      IsArchived       { get; set; }
    public DateTime? ArchivedAt       { get; set; }
    public int?      ArchivedByUserId { get; set; }

    public List<UserRole> UserRoles { get; set; } = new();
    public Supplier?       Supplier { get; set; }
}
