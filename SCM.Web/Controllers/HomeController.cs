using Microsoft.AspNetCore.Mvc;
using SCM.Application.Procurement.Interfaces;
using SCM.Application.Suppliers.Interfaces;
using SCM.Web.ViewModels;

namespace SCM.Web.Controllers;

public class HomeController : Controller
{
    private readonly ISupplierService _suppliers;
    private readonly IPurchaseOrderService _orders;

    public HomeController(ISupplierService suppliers, IPurchaseOrderService orders)
    {
        _suppliers = suppliers;
        _orders = orders;
    }

    public async Task<IActionResult> Index()
    {
        var supplierList = await _suppliers.GetAllAsync();
        var orderList = await _orders.GetAllAsync();

        var openStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Draft", "Sent", "PartiallyDelivered" };

        var delivered = orderList.Count(o => o.StatusName.Equals("Delivered", StringComparison.OrdinalIgnoreCase));
        var total = orderList.Count;
        var fulfillmentRate = total > 0 ? Math.Round((decimal)delivered / total * 100, 1) : 0m;

        var alerts = new List<DashboardAlertViewModel>();

        var pendingVerification = supplierList.Count(s =>
            !s.VerificationStatus.Equals("Verified", StringComparison.OrdinalIgnoreCase));
        if (pendingVerification > 0)
        {
            alerts.Add(new DashboardAlertViewModel
            {
                Title = $"{pendingVerification} supplier{(pendingVerification > 1 ? "s" : "")} pending verification",
                Description = "Review supplier credentials before issuing new POs.",
                Severity = pendingVerification > 2 ? "danger" : "warning",
                LinkUrl = Url.Action("Index", "Supplier")
            });
        }

        var draftOrders = orderList.Count(o => o.StatusName.Equals("Draft", StringComparison.OrdinalIgnoreCase));
        if (draftOrders > 0)
        {
            alerts.Add(new DashboardAlertViewModel
            {
                Title = $"{draftOrders} draft purchase order{(draftOrders > 1 ? "s" : "")} awaiting dispatch",
                Description = "Send orders to suppliers to begin fulfillment.",
                Severity = "info",
                LinkUrl = Url.Action("Index", "PurchaseOrder")
            });
        }

        var partial = orderList.Count(o => o.StatusName.Equals("PartiallyDelivered", StringComparison.OrdinalIgnoreCase));
        if (partial > 0)
        {
            alerts.Add(new DashboardAlertViewModel
            {
                Title = $"{partial} order{(partial > 1 ? "s" : "")} partially received",
                Description = "Record remaining deliveries to close open POs.",
                Severity = "warning",
                LinkUrl = Url.Action("Index", "PurchaseOrder")
            });
        }

        if (alerts.Count == 0)
        {
            alerts.Add(new DashboardAlertViewModel
            {
                Title = "All systems operational",
                Description = "No critical alerts at this time.",
                Severity = "info"
            });
        }

        var vm = new DashboardViewModel
        {
            TotalSuppliers = supplierList.Count,
            ActiveSuppliers = supplierList.Count(s => s.StatusName.Equals("Active", StringComparison.OrdinalIgnoreCase)),
            TotalPurchaseOrders = total,
            OpenPurchaseOrders = orderList.Count(o => openStatuses.Contains(o.StatusName)),
            DraftOrders = draftOrders,
            SentOrders = orderList.Count(o => o.StatusName.Equals("Sent", StringComparison.OrdinalIgnoreCase)),
            PartiallyDelivered = partial,
            DeliveredOrders = delivered,
            FulfillmentRate = fulfillmentRate,
            Alerts = alerts,
            RecentOrders = orderList
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new DashboardRecentOrderViewModel
                {
                    Id = o.Id,
                    SupplierCompanyName = o.SupplierCompanyName,
                    StatusName = o.StatusName,
                    OrderDate = o.OrderDate
                })
                .ToList()
        };

        return View(vm);
    }
}
