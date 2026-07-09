using SCM.Domain.Common;
namespace SCM.Domain.Entities;

/// <summary>
/// Maps to ER table Suppliers.
/// StatusId (Active/Inactive, via StatusTypes) is the operational gate for
/// BR-05 (only active suppliers may receive purchase orders).
/// VerificationStatus is a separate concept: an admin's assessment of the
/// supplier's business legitimacy (Pending/Verified/Rejected), stored as a
/// plain column rather than through StatusTypes.
/// </summary>
public class Supplier : BaseEntity, IArchivable
{
    public int      UserId                  { get; set; }
    public User      User                   { get; set; } = default!;

    public string   CompanyName             { get; set; } = default!;
    public string?  ContactEmail            { get; set; }
    public string?  ContactPhone            { get; set; }
    public string?  Address                 { get; set; }
    public string?  TaxIdentificationNumber { get; set; }
    public string?  BusinessLicenseNumber   { get; set; }

    public string   VerificationStatus      { get; set; } = "Pending"; // Pending | Verified | Rejected
    public DateTime? VerifiedAt             { get; set; }
    public int?      VerifiedByUserId       { get; set; }

    public int       StatusId               { get; set; } // FK -> StatusTypes (EntityType=Supplier). BR-05.
    public StatusType Status                { get; set; } = default!;

    public decimal?  PerformanceRating      { get; set; } // decimal(3,2)

    public bool      IsArchived             { get; set; }
    public DateTime? ArchivedAt             { get; set; }
    public int?      ArchivedByUserId       { get; set; }

    public DateTime  CreatedAt              { get; set; }
}
