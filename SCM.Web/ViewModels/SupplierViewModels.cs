using System.ComponentModel.DataAnnotations;

namespace SCM.Web.ViewModels;

public class SupplierListItemViewModel
{
    public int      Id                 { get; set; }
    public string   CompanyName        { get; set; } = default!;
    public string?  ContactEmail       { get; set; }
    public string   VerificationStatus { get; set; } = default!;
    public string   StatusName         { get; set; } = default!;
    public decimal? PerformanceRating  { get; set; }
}

public class SupplierDetailViewModel
{
    public int      Id                      { get; set; }
    public string   FullName                { get; set; } = default!;
    public string   Email                   { get; set; } = default!;
    public string   CompanyName             { get; set; } = default!;
    public string?  ContactEmail            { get; set; }
    public string?  ContactPhone            { get; set; }
    public string?  Address                 { get; set; }
    public string?  TaxIdentificationNumber { get; set; }
    public string?  BusinessLicenseNumber   { get; set; }
    public string   VerificationStatus      { get; set; } = default!;
    public string   StatusName              { get; set; } = default!;
    public decimal? PerformanceRating       { get; set; }
    public DateTime CreatedAt               { get; set; }
}

public class SupplierRegisterViewModel
{
    [Required, Display(Name = "Full name")]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required, DataType(DataType.Password), MinLength(8)]
    public string Password { get; set; } = default!;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = default!;

    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Required, Display(Name = "Company name")]
    public string CompanyName { get; set; } = default!;

    [EmailAddress, Display(Name = "Contact email")]
    public string? ContactEmail { get; set; }

    [Display(Name = "Contact phone")]
    public string? ContactPhone { get; set; }

    public string? Address { get; set; }

    [Display(Name = "Tax ID number")]
    public string? TaxIdentificationNumber { get; set; }

    [Display(Name = "Business license number")]
    public string? BusinessLicenseNumber { get; set; }
}

public class SupplierEditViewModel
{
    public int Id { get; set; }

    [Required, Display(Name = "Company name")]
    public string CompanyName { get; set; } = default!;

    [EmailAddress, Display(Name = "Contact email")]
    public string? ContactEmail { get; set; }

    [Display(Name = "Contact phone")]
    public string? ContactPhone { get; set; }

    public string? Address { get; set; }

    [Display(Name = "Tax ID number")]
    public string? TaxIdentificationNumber { get; set; }

    [Display(Name = "Business license number")]
    public string? BusinessLicenseNumber { get; set; }
}
