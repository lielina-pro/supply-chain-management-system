using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(ScmDbContext context) : base(context) { }

    public async Task<PurchaseOrder?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        await Set.Include(o => o.Items).ThenInclude(i => i.Product)
                  .Include(o => o.Supplier)
                  .Include(o => o.Status)
                  .Include(o => o.PurchaseRequest)
                  .FirstOrDefaultAsync(o => o.Id == id && !o.IsArchived, ct);

    public async Task<IReadOnlyList<PurchaseOrder>> GetAllWithDetailsAsync(CancellationToken ct = default) =>
        await Set.Include(o => o.Items).ThenInclude(i => i.Product)
                  .Include(o => o.Supplier)
                  .Include(o => o.Status)
                  .Where(o => !o.IsArchived)
                  .OrderByDescending(o => o.OrderDate)
                  .ToListAsync(ct);
}
