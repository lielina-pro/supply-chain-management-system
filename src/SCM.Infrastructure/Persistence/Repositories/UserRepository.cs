using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;
using SCM.Domain.Interfaces.Repositories;

namespace SCM.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ScmDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await Set.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await Set.AnyAsync(u => u.Email == email, ct);
}
