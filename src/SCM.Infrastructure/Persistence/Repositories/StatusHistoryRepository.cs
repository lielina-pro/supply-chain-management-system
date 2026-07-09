using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class StatusHistoryRepository : Repository<StatusHistory>, IStatusHistoryRepository
{
    public StatusHistoryRepository(ScmDbContext context) : base(context) { }

    public async Task<IReadOnlyList<StatusHistory>> GetForEntityAsync(string entityType, int entityId, CancellationToken ct = default) =>
        await Set.Where(s => s.EntityType == entityType && s.EntityId == entityId)
                  .OrderBy(s => s.ChangedAt)
                  .ToListAsync(ct);
}
