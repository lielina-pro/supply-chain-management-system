using System.ComponentModel.DataAnnotations;
namespace SCM.Application.Suppliers.DTOs;

/// <summary>FR-02.2 - Update an existing supplier profile.</summary>
public class UpdateSupplierProfileRequest
{
    [Required, MaxLength(200)]
    public string CompanyName { get; set; } = default!;

    [EmailAddress]
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? TaxIdentificationNumber { get; set; }
    public string? BusinessLicenseNumber { get; set; }
}
