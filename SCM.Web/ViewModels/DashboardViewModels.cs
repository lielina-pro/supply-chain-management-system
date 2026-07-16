namespace SCM.Web.ViewModels;

public class DashboardViewModel
{
    public int TotalSuppliers { get; set; }
    public int ActiveSuppliers { get; set; }
    public int TotalPurchaseOrders { get; set; }
    public int OpenPurchaseOrders { get; set; }
    public int DraftOrders { get; set; }
    public int SentOrders { get; set; }
    public int PartiallyDelivered { get; set; }
    public int DeliveredOrders { get; set; }
    public decimal FulfillmentRate { get; set; }
    public List<DashboardAlertViewModel> Alerts { get; set; } = [];
    public List<DashboardRecentOrderViewModel> RecentOrders { get; set; } = [];
}

public class DashboardAlertViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Severity { get; set; } = "info"; // danger, warning, info
    public string? LinkUrl { get; set; }
}

public class DashboardRecentOrderViewModel
{
    public int Id { get; set; }
    public string SupplierCompanyName { get; set; } = "";
    public string StatusName { get; set; } = "";
    public DateTime OrderDate { get; set; }
}
