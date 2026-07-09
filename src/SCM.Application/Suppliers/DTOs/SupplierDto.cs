namespace SCM.Application.Suppliers.DTOs;

/// <summary>Full supplier profile - used by the detail view and API GetById.</summary>
public class SupplierDto
{
    public int      Id                      { get; set; }
    public int      UserId                  { get; set; }
    public string   FullName                { get; set; } = default!;
    public string   Email                    { get; set; } = default!;
    public string   CompanyName             { get; set; } = default!;
    public string?  ContactEmail            { get; set; }
    public string?  ContactPhone            { get; set; }
    public string?  Address                 { get; set; }
    public string?  TaxIdentificationNumber { get; set; }
    public string?  BusinessLicenseNumber   { get; set; }
    public string   VerificationStatus      { get; set; } = default!;
    public string   StatusName              { get; set; } = default!; // Active | Inactive
    public decimal? PerformanceRating       { get; set; }
    public DateTime CreatedAt               { get; set; }
}
