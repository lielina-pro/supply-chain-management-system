using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    /// <summary>Loads a PO with Items (+Product), Supplier, Status, and the source PurchaseRequest included.</summary>
    Task<PurchaseOrder?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<PurchaseOrder>> GetAllWithDetailsAsync(CancellationToken ct = default);
}
