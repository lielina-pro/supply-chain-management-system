using SCM.Application.Common.Models;

namespace SCM.Application.Orders.Interfaces;

// Placeholder so OrdersController compiles. Not implemented/registered yet -
// this is the Week 6 module (FR-06).
public interface IOrderService
{
    Task<ServiceResult<object>> PlaceOrderAsync(object req, CancellationToken ct = default);
    Task<object?>                GetOrderStatusAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<object>> CancelOrderAsync(int id, CancellationToken ct = default);
}
