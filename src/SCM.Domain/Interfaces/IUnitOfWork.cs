using SCM.Domain.Interfaces.Repositories;
namespace SCM.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ISupplierRepository     Suppliers     { get; }
    IUserRepository         Users         { get; }
    IRoleRepository         Roles         { get; }
    IStatusTypeRepository   StatusTypes   { get; }
    IStatusHistoryRepository StatusHistory { get; }
    IAuditLogRepository     AuditLogs     { get; }

    // Inventory/Orders/Procurement unit-of-work members are added when those
    // modules' entities are modeled in later weeks.

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
