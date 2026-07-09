using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary> Maps to ER table Roles. The 8 system roles (FR-01.2) are seeded rows, not an enum. </summary>
public class Role : BaseEntity, IArchivable
{
    public string  Name        { get; set; } = default!;
    public string? Description { get; set; }
    public bool    IsActive    { get; set; } = true;
    public DateTime CreatedAt  { get; set; }

    public bool      IsArchived       { get; set; }
    public DateTime? ArchivedAt       { get; set; }
    public int?      ArchivedByUserId { get; set; }

    public List<UserRole> UserRoles { get; set; } = new();
}

/// <summary> Well-known role names, used for seeding and RBAC policy checks (avoids magic strings). </summary>
public static class RoleNames
{
    public const string Administrator        = "Administrator";
    public const string ProcurementManager    = "ProcurementManager";
    public const string WarehouseManager      = "WarehouseManager";
    public const string LogisticsCoordinator  = "LogisticsCoordinator";
    public const string SalesManager          = "SalesManager";
    public const string FinanceAnalyst        = "FinanceAnalyst";
    public const string Supplier              = "Supplier";
    public const string Customer              = "Customer";

    public static readonly string[] All =
    {
        Administrator, ProcurementManager, WarehouseManager, LogisticsCoordinator,
        SalesManager, FinanceAnalyst, Supplier, Customer
    };
}
