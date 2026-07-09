using Microsoft.AspNetCore.Mvc;
using SCM.Application.Suppliers.DTOs;
using SCM.Application.Suppliers.Interfaces;
using SCM.Web.ViewModels;

namespace SCM.Web.Controllers;

public class SupplierController : Controller
{
    private readonly ISupplierService _svc;
    public SupplierController(ISupplierService svc) => _svc = svc;

    // GET /Supplier — FR-02.5 supplier list
    public async Task<IActionResult> Index()
    {
        var suppliers = await _svc.GetAllAsync();
        var vm = suppliers.Select(s => new SupplierListItemViewModel
        {
            Id                 = s.Id,
            CompanyName        = s.CompanyName,
            ContactEmail       = s.ContactEmail,
            VerificationStatus = s.VerificationStatus,
            StatusName         = s.StatusName,
            PerformanceRating  = s.PerformanceRating
        }).ToList();
        return View(vm);
    }

    // GET /Supplier/Details/5 — FR-02.2 profile
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        if (dto is null) return NotFound();

        return View(new SupplierDetailViewModel
        {
            Id                      = dto.Id,
            FullName                = dto.FullName,
            Email                   = dto.Email,
            CompanyName             = dto.CompanyName,
            ContactEmail            = dto.ContactEmail,
            ContactPhone            = dto.ContactPhone,
            Address                 = dto.Address,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            BusinessLicenseNumber   = dto.BusinessLicenseNumber,
            VerificationStatus      = dto.VerificationStatus,
            StatusName              = dto.StatusName,
            PerformanceRating       = dto.PerformanceRating,
            CreatedAt               = dto.CreatedAt
        });
    }

    // GET /Supplier/Register — FR-02.1 self-registration form
    [HttpGet]
    public IActionResult Register() => View(new SupplierRegisterViewModel());

    // POST /Supplier/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(SupplierRegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _svc.RegisterAsync(new CreateSupplierRequest
        {
            FullName                = vm.FullName,
            Email                   = vm.Email,
            Password                = vm.Password,
            PhoneNumber             = vm.PhoneNumber,
            CompanyName             = vm.CompanyName,
            ContactEmail            = vm.ContactEmail,
            ContactPhone            = vm.ContactPhone,
            Address                 = vm.Address,
            TaxIdentificationNumber = vm.TaxIdentificationNumber,
            BusinessLicenseNumber   = vm.BusinessLicenseNumber
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(vm);
        }

        TempData["SuccessMessage"] = $"Supplier '{result.Value!.CompanyName}' registered successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    // GET /Supplier/Edit/5 — FR-02.2 profile update
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        if (dto is null) return NotFound();

        return View(new SupplierEditViewModel
        {
            Id                      = dto.Id,
            CompanyName             = dto.CompanyName,
            ContactEmail            = dto.ContactEmail,
            ContactPhone            = dto.ContactPhone,
            Address                 = dto.Address,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            BusinessLicenseNumber   = dto.BusinessLicenseNumber
        });
    }

    // POST /Supplier/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierEditViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var result = await _svc.UpdateProfileAsync(id, new UpdateSupplierProfileRequest
        {
            CompanyName             = vm.CompanyName,
            ContactEmail            = vm.ContactEmail,
            ContactPhone            = vm.ContactPhone,
            Address                 = vm.Address,
            TaxIdentificationNumber = vm.TaxIdentificationNumber,
            BusinessLicenseNumber   = vm.BusinessLicenseNumber
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(vm);
        }

        TempData["SuccessMessage"] = "Supplier profile updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Supplier/ChangeStatus/5 — FR-02.4 (BR-05 gate). ProcurementManager/Admin action from the Details view.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, string statusName)
    {
        // TODO: pass the acting user's id once MVC auth (cookie/JWT) is wired up.
        var result = await _svc.UpdateStatusAsync(id, new UpdateSupplierStatusRequest { StatusName = statusName }, actingUserId: null);

        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
            result.IsSuccess ? $"Supplier status changed to {statusName}." : result.Error;

        return RedirectToAction(nameof(Details), new { id });
    }
}
