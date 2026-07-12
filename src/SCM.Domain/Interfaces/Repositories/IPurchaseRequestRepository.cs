using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IPurchaseRequestRepository : IRepository<PurchaseRequest>
{
    /// <summary>Loads a request with Items (+Product), Approvals, Status, and requester included.</summary>
    Task<PurchaseRequest?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<PurchaseRequest>> GetAllWithDetailsAsync(CancellationToken ct = default);
}
