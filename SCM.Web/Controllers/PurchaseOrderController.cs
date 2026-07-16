using Microsoft.AspNetCore.Mvc;
using SCM.Application.Procurement.DTOs;
using SCM.Application.Procurement.Interfaces;
using SCM.Web.ViewModels;

namespace SCM.Web.Controllers;

public class PurchaseOrderController : Controller
{
    private readonly IPurchaseOrderService _svc;
    public PurchaseOrderController(IPurchaseOrderService svc) => _svc = svc;

    // GET /PurchaseOrder — FR-03.4 tracking list
    public async Task<IActionResult> Index()
    {
        var pos = await _svc.GetAllAsync();
        var vm = pos.Select(o => new PurchaseOrderListItemViewModel
        {
            Id = o.Id, SupplierCompanyName = o.SupplierCompanyName,
            StatusName = o.StatusName, OrderDate = o.OrderDate, TotalAmount = o.TotalAmount
        }).ToList();
        return View(vm);
    }

    // GET /PurchaseOrder/Details/5 — FR-03.4 tracking detail + FR-03.5 receiving form
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _svc.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return View(MapDetail(dto));
    }

    // GET /PurchaseOrder/Create — FR-03.3
    [HttpGet]
    public IActionResult Create() => View(new CreatePurchaseOrderViewModel());

    // POST /PurchaseOrder/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseOrderViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _svc.CreateAsync(new CreatePurchaseOrderRequest
        {
            PurchaseRequestId = vm.PurchaseRequestId,
            SupplierId = vm.SupplierId,
            ExpectedDeliveryDate = vm.ExpectedDeliveryDate
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(vm);
        }

        TempData["SuccessMessage"] = $"Purchase order #{result.Value!.Id} generated (Draft).";
        return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
    }

    // POST /PurchaseOrder/Send/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int id)
    {
        var result = await _svc.SendToSupplierAsync(id);
        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
            result.IsSuccess ? "Purchase order sent to supplier." : result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /PurchaseOrder/RecordDelivery/5 — FR-03.5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordDelivery(RecordDeliveryViewModel vm)
    {
        var req = new RecordDeliveryRequest
        {
            WarehouseId = vm.WarehouseId,
            Lines = vm.Lines
                .Where(l => l.QuantityReceived > 0)
                .Select(l => new RecordDeliveryLineInput { PurchaseOrderItemId = l.PurchaseOrderItemId, QuantityReceived = l.QuantityReceived })
                .ToList()
        };

        // TODO: pass the real acting user's id once MVC auth (cookie/JWT) is wired up.
        var result = await _svc.RecordDeliveryAsync(vm.PurchaseOrderId, req, performedByUserId: 1);

        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
            result.IsSuccess ? "Delivery recorded." : result.Error;
        return RedirectToAction(nameof(Details), new { id = vm.PurchaseOrderId });
    }

    private static PurchaseOrderDetailViewModel MapDetail(PurchaseOrderDto dto) => new()
    {
        Id = dto.Id, PurchaseRequestId = dto.PurchaseRequestId,
        SupplierCompanyName = dto.SupplierCompanyName, StatusName = dto.StatusName,
        OrderDate = dto.OrderDate, ExpectedDeliveryDate = dto.ExpectedDeliveryDate, TotalAmount = dto.TotalAmount,
        Items = dto.Items.Select(i => new PurchaseOrderItemViewModel
        {
            Id = i.Id, ProductName = i.ProductName, SKU = i.SKU,
            QuantityOrdered = i.QuantityOrdered, QuantityReceived = i.QuantityReceived, UnitPrice = i.UnitPrice
        }).ToList()
    };
}
