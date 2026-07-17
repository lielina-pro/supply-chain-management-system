namespace SCM.Application.Suppliers.DTOs;

public class VerifySupplierRequest
{
    public string  Decision { get; set; } = default!; // Verified | Rejected
    public string? Notes    { get; set; }
}