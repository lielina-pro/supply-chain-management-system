using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence;

public class ScmDbContext : DbContext
{
    public ScmDbContext(DbContextOptions<ScmDbContext> options) : base(options) { }

    public DbSet<User>          Users         => Set<User>();
    public DbSet<Role>          Roles         => Set<Role>();
    public DbSet<UserRole>      UserRoles     => Set<UserRole>();
    public DbSet<StatusType>    StatusTypes   => Set<StatusType>();
    public DbSet<StatusHistory> StatusHistory => Set<StatusHistory>();
    public DbSet<AuditLog>      AuditLogs     => Set<AuditLog>();
    public DbSet<Supplier>      Suppliers     => Set<Supplier>();

    // Procurement module (Week 4)
    public DbSet<Product>                 Products                 => Set<Product>();
    public DbSet<PurchaseRequest>         PurchaseRequests         => Set<PurchaseRequest>();
    public DbSet<PurchaseRequestItem>     PurchaseRequestItems     => Set<PurchaseRequestItem>();
    public DbSet<PurchaseRequestApproval> PurchaseRequestApprovals => Set<PurchaseRequestApproval>();
    public DbSet<PurchaseOrder>           PurchaseOrders           => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem>       PurchaseOrderItems       => Set<PurchaseOrderItem>();

    // Delivery/inventory support for FR-03.5 (BR-04)
    public DbSet<Warehouse>        Warehouses        => Set<Warehouse>();
    public DbSet<InventoryStock>   InventoryStock    => Set<InventoryStock>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    // Other modules' DbSets (CustomerOrders, Shipments, Payments, Notifications,
    // DemandForecasts, Racks, Bins, ...) are added as each module is built in
    // later weeks, per the 12-week plan.

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(ScmDbContext).Assembly);
        base.OnModelCreating(mb);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}
