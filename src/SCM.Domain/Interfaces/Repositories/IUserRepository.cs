using SCM.Domain.Entities;
namespace SCM.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  EmailExistsAsync(string email, CancellationToken ct = default);
}
