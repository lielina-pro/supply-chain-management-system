using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class InventoryStockRepository : Repository<InventoryStock>, IInventoryStockRepository
{
    public InventoryStockRepository(ScmDbContext context) : base(context) { }

    public async Task<InventoryStock?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken ct = default) =>
        await Set.FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId, ct);
}
