using SCM.Application.Common.Models;
using SCM.Application.Suppliers.DTOs;

namespace SCM.Application.Suppliers.Interfaces;

public interface ISupplierService
{
    /// <summary>FR-02.1 - Register a new supplier (creates linked User + Supplier).</summary>
    Task<ServiceResult<SupplierDto>> RegisterAsync(CreateSupplierRequest req, CancellationToken ct = default);

    /// <summary>FR-02.2 - Read a supplier profile.</summary>
    Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>FR-02.5 - Supplier list / history.</summary>
    Task<IReadOnlyList<SupplierListItemDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>FR-02.2 - Update an existing supplier profile.</summary>
    Task<ServiceResult<SupplierDto>> UpdateProfileAsync(int id, UpdateSupplierProfileRequest req, CancellationToken ct = default);

    /// <summary>FR-02.4 - Activate/deactivate a supplier (BR-05 gate). Records a StatusHistory row.</summary>
    Task<ServiceResult<bool>> UpdateStatusAsync(int id, UpdateSupplierStatusRequest req, int? actingUserId, CancellationToken ct = default);

    /// <summary>FR-02.7 - Verify or reject a supplier (Admin only).</summary>
    Task<ServiceResult<SupplierDto>> VerifyAsync(int id, VerifySupplierRequest req, int actingUserId, CancellationToken ct = default);
}