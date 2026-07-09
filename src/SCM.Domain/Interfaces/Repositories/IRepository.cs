using System.Linq.Expressions;
namespace SCM.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?>             GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> pred, CancellationToken ct = default);
    Task                 AddAsync(T entity, CancellationToken ct = default);
    void                 Update(T entity);
    void                 Remove(T entity);
}
