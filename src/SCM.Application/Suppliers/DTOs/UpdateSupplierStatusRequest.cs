using System.ComponentModel.DataAnnotations;
namespace SCM.Application.Suppliers.DTOs;

/// <summary>FR-02.4 - Activate/deactivate a supplier (gates BR-05).</summary>
public class UpdateSupplierStatusRequest
{
    [Required]
    public string StatusName { get; set; } = default!; // "Active" | "Inactive"
    public string? Notes { get; set; }
}
