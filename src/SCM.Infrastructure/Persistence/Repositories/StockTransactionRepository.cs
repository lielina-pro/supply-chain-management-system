using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class StockTransactionRepository : Repository<StockTransaction>, IStockTransactionRepository
{
    public StockTransactionRepository(ScmDbContext context) : base(context) { }
}
