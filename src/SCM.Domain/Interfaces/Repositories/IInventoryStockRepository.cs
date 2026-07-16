using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IInventoryStockRepository : IRepository<InventoryStock>
{
    Task<InventoryStock?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken ct = default);
}
