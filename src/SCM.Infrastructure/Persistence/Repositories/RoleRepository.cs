using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ScmDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await Set.FirstOrDefaultAsync(r => r.Name == name, ct);
}
