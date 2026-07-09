using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IStatusHistoryRepository : IRepository<StatusHistory>
{
    Task<IReadOnlyList<StatusHistory>> GetForEntityAsync(string entityType, int entityId, CancellationToken ct = default);
}
