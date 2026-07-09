namespace SCM.Application.Suppliers.DTOs;

/// <summary>Lightweight row shape for the supplier list view (FR-02.5 supplier history / list).</summary>
public class SupplierListItemDto
{
    public int      Id                 { get; set; }
    public string   CompanyName        { get; set; } = default!;
    public string?  ContactEmail       { get; set; }
    public string   VerificationStatus { get; set; } = default!;
    public string   StatusName         { get; set; } = default!;
    public decimal? PerformanceRating  { get; set; }
}
