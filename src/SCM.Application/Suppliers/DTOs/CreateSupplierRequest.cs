using System.ComponentModel.DataAnnotations;
namespace SCM.Application.Suppliers.DTOs;

/// <summary>
/// FR-02.1 - Supplier self-registration. Creates the linked User account
/// (with the Supplier role, satisfying BR-09) and the Supplier profile
/// together, atomically.
/// </summary>
public class CreateSupplierRequest
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required, MinLength(8)]
    public string Password { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = default!;

    [EmailAddress]
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? TaxIdentificationNumber { get; set; }
    public string? BusinessLicenseNumber { get; set; }
}
