using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ScmDbContext context) : base(context) { }

    public async Task<Supplier?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        await Set.Include(s => s.User)
                  .Include(s => s.Status)
                  .FirstOrDefaultAsync(s => s.Id == id && !s.IsArchived, ct);

    public async Task<IReadOnlyList<Supplier>> GetAllWithDetailsAsync(CancellationToken ct = default) =>
        await Set.Include(s => s.User)
                  .Include(s => s.Status)
                  .Where(s => !s.IsArchived)
                  .OrderBy(s => s.CompanyName)
                  .ToListAsync(ct);

    public async Task<Supplier?> GetByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Set.Include(s => s.Status).FirstOrDefaultAsync(s => s.UserId == userId, ct);

    public async Task<bool> ExistsForUserAsync(int userId, CancellationToken ct = default) =>
        await Set.AnyAsync(s => s.UserId == userId, ct);
}
