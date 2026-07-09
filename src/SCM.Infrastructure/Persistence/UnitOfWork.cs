using SCM.Domain.Interfaces;
using SCM.Domain.Interfaces.Repositories;
using SCM.Infrastructure.Persistence.Repositories;

namespace SCM.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ScmDbContext _context;

    public UnitOfWork(ScmDbContext context)
    {
        _context      = context;
        Suppliers     = new SupplierRepository(context);
        Users         = new UserRepository(context);
        Roles         = new RoleRepository(context);
        StatusTypes   = new StatusTypeRepository(context);
        StatusHistory = new StatusHistoryRepository(context);
        AuditLogs     = new AuditLogRepository(context);
    }

    public ISupplierRepository      Suppliers     { get; }
    public IUserRepository          Users         { get; }
    public IRoleRepository          Roles         { get; }
    public IStatusTypeRepository    StatusTypes   { get; }
    public IStatusHistoryRepository StatusHistory { get; }
    public IAuditLogRepository      AuditLogs     { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
