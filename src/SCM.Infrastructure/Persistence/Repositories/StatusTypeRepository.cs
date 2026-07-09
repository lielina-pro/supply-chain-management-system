using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class StatusTypeRepository : Repository<StatusType>, IStatusTypeRepository
{
    public StatusTypeRepository(ScmDbContext context) : base(context) { }

    public async Task<StatusType?> GetByNameAsync(string entityType, string statusName, CancellationToken ct = default) =>
        await Set.FirstOrDefaultAsync(s => s.EntityType == entityType && s.StatusName == statusName, ct);

    public async Task<IReadOnlyList<StatusType>> GetForEntityTypeAsync(string entityType, CancellationToken ct = default) =>
        await Set.Where(s => s.EntityType == entityType && !s.IsArchived).ToListAsync(ct);
}
