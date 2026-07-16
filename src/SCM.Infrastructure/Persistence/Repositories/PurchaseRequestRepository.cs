using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class PurchaseRequestRepository : Repository<PurchaseRequest>, IPurchaseRequestRepository
{
    public PurchaseRequestRepository(ScmDbContext context) : base(context) { }

    public async Task<PurchaseRequest?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        await Set.Include(r => r.Items).ThenInclude(i => i.Product)
                  .Include(r => r.Approvals).ThenInclude(a => a.ApproverUser)
                  .Include(r => r.Status)
                  .Include(r => r.RequestedByUser)
                  .FirstOrDefaultAsync(r => r.Id == id && !r.IsArchived, ct);

    public async Task<IReadOnlyList<PurchaseRequest>> GetAllWithDetailsAsync(CancellationToken ct = default) =>
        await Set.Include(r => r.Items).ThenInclude(i => i.Product)
                  .Include(r => r.Status)
                  .Include(r => r.RequestedByUser)
                  .Where(r => !r.IsArchived)
                  .OrderByDescending(r => r.CreatedAt)
                  .ToListAsync(ct);
}
