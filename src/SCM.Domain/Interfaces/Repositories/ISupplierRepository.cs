using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    /// <summary>Loads a supplier with its linked User and Status included (for detail views).</summary>
    Task<Supplier?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    /// <summary>All non-archived suppliers with User and Status included (for list views).</summary>
    Task<IReadOnlyList<Supplier>> GetAllWithDetailsAsync(CancellationToken ct = default);

    Task<Supplier?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<bool>      ExistsForUserAsync(int userId, CancellationToken ct = default);
}
