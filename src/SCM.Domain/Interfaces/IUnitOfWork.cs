using SCM.Domain.Interfaces.Repositories;
namespace SCM.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ISupplierRepository     Suppliers     { get; }
    IUserRepository         Users         { get; }
    IRoleRepository         Roles         { get; }
    IStatusTypeRepository   StatusTypes   { get; }
    IStatusHistoryRepository StatusHistory { get; }
    IAuditLogRepository     AuditLogs     { get; }

    // Procurement module (Week 4)
    IProductRepository         Products         { get; }
    IPurchaseRequestRepository PurchaseRequests { get; }
    IPurchaseOrderRepository   PurchaseOrders   { get; }

    // Delivery/inventory support for FR-03.5 (BR-04)
    IWarehouseRepository        Warehouses        { get; }
    IInventoryStockRepository   InventoryStocks   { get; }
    IStockTransactionRepository StockTransactions { get; }

    // Inventory/Orders unit-of-work members are added when those modules'
    // entities are modeled in later weeks.

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
