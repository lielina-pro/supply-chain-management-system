using SCM.Application.Common.Exceptions;
using SCM.Application.Common.Interfaces;
using SCM.Application.Common.Models;
using SCM.Application.Suppliers.DTOs;
using SCM.Application.Suppliers.Interfaces;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces;

namespace SCM.Application.Suppliers.Services;

public class SupplierService : ISupplierService
{
    private const string EntityType = "Supplier";
    private static readonly string[] ValidStatuses = { "Active", "Inactive" };

    private readonly IUnitOfWork      _uow;
    private readonly IPasswordHasher  _hasher;

    public SupplierService(IUnitOfWork uow, IPasswordHasher hasher)
    {
        _uow = uow;
        _hasher = hasher;
    }

    public async Task<ServiceResult<SupplierDto>> RegisterAsync(CreateSupplierRequest req, CancellationToken ct = default)
    {
        if (await _uow.Users.EmailExistsAsync(req.Email, ct))
            return ServiceResult<SupplierDto>.Failure("A user with this email already exists.");

        var supplierRole = await _uow.Roles.GetByNameAsync(Domain.Entities.RoleNames.Supplier, ct)
            ?? throw new BusinessRuleException("Supplier role is not seeded. Run database seed first.");

        var activeStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, "Active", ct)
            ?? throw new BusinessRuleException("Supplier status types are not seeded. Run database seed first.");

        var now = DateTime.UtcNow;

        var user = new User
        {
            FullName     = req.FullName,
            Email        = req.Email,
            PasswordHash = _hasher.Hash(req.Password),
            PhoneNumber  = req.PhoneNumber,
            IsActive     = true,
            CreatedAt    = now
        };
        user.UserRoles.Add(new UserRole { User = user, Role = supplierRole, AssignedAt = now }); // BR-09

        var supplier = new Supplier
        {
            User                    = user, // FK fix-up on save
            CompanyName              = req.CompanyName,
            ContactEmail             = req.ContactEmail,
            ContactPhone             = req.ContactPhone,
            Address                  = req.Address,
            TaxIdentificationNumber  = req.TaxIdentificationNumber,
            BusinessLicenseNumber    = req.BusinessLicenseNumber,
            VerificationStatus       = "Pending",
            Status                   = activeStatus, // FK fix-up on save; new suppliers start Active
            CreatedAt                = now
        };

        await _uow.Users.AddAsync(user, ct);
        await _uow.Suppliers.AddAsync(supplier, ct);
        await _uow.SaveChangesAsync(ct); // assigns Ids via fix-up

        await _uow.StatusHistory.AddAsync(new StatusHistory
        {
            EntityType      = EntityType,
            EntityId        = supplier.Id,
            FromStatusId    = null,
            ToStatusId      = activeStatus.Id,
            ChangedByUserId = null,
            ChangedAt       = now,
            Notes           = "Initial registration"
        }, ct);
        await _uow.SaveChangesAsync(ct);

        return ServiceResult<SupplierDto>.Success(MapToDto(supplier, user, activeStatus));
    }

    public async Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var supplier = await _uow.Suppliers.GetWithDetailsAsync(id, ct);
        return supplier is null ? null : MapToDto(supplier, supplier.User, supplier.Status);
    }

    public async Task<IReadOnlyList<SupplierListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var suppliers = await _uow.Suppliers.GetAllWithDetailsAsync(ct);
        return suppliers.Select(s => new SupplierListItemDto
        {
            Id                 = s.Id,
            CompanyName        = s.CompanyName,
            ContactEmail       = s.ContactEmail,
            VerificationStatus = s.VerificationStatus,
            StatusName         = s.Status.StatusName,
            PerformanceRating  = s.PerformanceRating
        }).ToList();
    }

    public async Task<ServiceResult<SupplierDto>> UpdateProfileAsync(int id, UpdateSupplierProfileRequest req, CancellationToken ct = default)
    {
        var supplier = await _uow.Suppliers.GetWithDetailsAsync(id, ct);
        if (supplier is null)
            return ServiceResult<SupplierDto>.Failure("Supplier not found.");

        supplier.CompanyName             = req.CompanyName;
        supplier.ContactEmail            = req.ContactEmail;
        supplier.ContactPhone            = req.ContactPhone;
        supplier.Address                 = req.Address;
        supplier.TaxIdentificationNumber = req.TaxIdentificationNumber;
        supplier.BusinessLicenseNumber   = req.BusinessLicenseNumber;

        _uow.Suppliers.Update(supplier);
        await _uow.SaveChangesAsync(ct);

        return ServiceResult<SupplierDto>.Success(MapToDto(supplier, supplier.User, supplier.Status));
    }

    public async Task<ServiceResult<bool>> UpdateStatusAsync(int id, UpdateSupplierStatusRequest req, int? actingUserId, CancellationToken ct = default)
    {
        if (!ValidStatuses.Contains(req.StatusName, StringComparer.OrdinalIgnoreCase))
            return ServiceResult<bool>.Failure("Status must be 'Active' or 'Inactive'.");

        var supplier = await _uow.Suppliers.GetWithDetailsAsync(id, ct);
        if (supplier is null)
            return ServiceResult<bool>.Failure("Supplier not found.");

        var targetStatus = await _uow.StatusTypes.GetByNameAsync(EntityType, req.StatusName, ct)
            ?? throw new BusinessRuleException($"Status type '{req.StatusName}' is not seeded for Supplier.");

        if (supplier.StatusId == targetStatus.Id)
            return ServiceResult<bool>.Success(true); // already in that status, no-op

        var previousStatusId = supplier.StatusId;
        var now = DateTime.UtcNow;

        supplier.StatusId = targetStatus.Id;
        _uow.Suppliers.Update(supplier);

        await _uow.StatusHistory.AddAsync(new StatusHistory
        {
            EntityType      = EntityType,
            EntityId        = supplier.Id,
            FromStatusId    = previousStatusId,
            ToStatusId      = targetStatus.Id,
            ChangedByUserId = actingUserId,
            ChangedAt       = now,
            Notes           = req.Notes
        }, ct);

        await _uow.SaveChangesAsync(ct);
        return ServiceResult<bool>.Success(true);
    }

    private static SupplierDto MapToDto(Supplier s, User user, StatusType status) => new()
    {
        Id                      = s.Id,
        UserId                  = s.UserId,
        FullName                = user.FullName,
        Email                   = user.Email,
        CompanyName             = s.CompanyName,
        ContactEmail            = s.ContactEmail,
        ContactPhone            = s.ContactPhone,
        Address                 = s.Address,
        TaxIdentificationNumber = s.TaxIdentificationNumber,
        BusinessLicenseNumber   = s.BusinessLicenseNumber,
        VerificationStatus      = s.VerificationStatus,
        StatusName              = status.StatusName,
        PerformanceRating       = s.PerformanceRating,
        CreatedAt               = s.CreatedAt
    };
}
