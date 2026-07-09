using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IStatusTypeRepository : IRepository<StatusType>
{
    Task<StatusType?> GetByNameAsync(string entityType, string statusName, CancellationToken ct = default);
    Task<IReadOnlyList<StatusType>> GetForEntityTypeAsync(string entityType, CancellationToken ct = default);
}
