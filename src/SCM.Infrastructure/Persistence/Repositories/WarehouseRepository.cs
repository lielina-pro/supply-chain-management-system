using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ScmDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Warehouse>> GetAllActiveAsync(CancellationToken ct = default) =>
        await Set.Where(w => w.IsActive && !w.IsArchived).OrderBy(w => w.Name).ToListAsync(ct);

    public async Task<Warehouse?> GetDefaultAsync(CancellationToken ct = default) =>
        await Set.Where(w => w.IsActive && !w.IsArchived).OrderBy(w => w.Id).FirstOrDefaultAsync(ct);
}
