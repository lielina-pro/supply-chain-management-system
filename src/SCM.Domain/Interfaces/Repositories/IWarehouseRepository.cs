using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<IReadOnlyList<Warehouse>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Warehouse?> GetDefaultAsync(CancellationToken ct = default); // first active warehouse - demo convenience
}
