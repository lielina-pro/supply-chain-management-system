using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?>              GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllActiveAsync(CancellationToken ct = default);
}
