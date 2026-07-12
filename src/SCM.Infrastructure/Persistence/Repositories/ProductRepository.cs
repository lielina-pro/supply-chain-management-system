using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ScmDbContext context) : base(context) { }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default) =>
        await Set.FirstOrDefaultAsync(p => p.SKU == sku, ct);

    public async Task<IReadOnlyList<Product>> GetAllActiveAsync(CancellationToken ct = default) =>
        await Set.Where(p => p.IsActive && !p.IsArchived).OrderBy(p => p.Name).ToListAsync(ct);
}
