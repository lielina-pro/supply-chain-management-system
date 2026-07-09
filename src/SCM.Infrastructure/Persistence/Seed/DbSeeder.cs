using Microsoft.EntityFrameworkCore;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the 8 system roles (FR-01.2) and the StatusTypes rows the Supplier
/// module depends on. Call once at startup (see Program.cs). Idempotent.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(ScmDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Roles.AnyAsync())
        {
            var now = DateTime.UtcNow;
            db.Roles.AddRange(RoleNames.All.Select(name => new Role
            {
                Name      = name,
                IsActive  = true,
                CreatedAt = now
            }));
            await db.SaveChangesAsync();
        }

        // Bootstrap admin user (CreatedBy for StatusTypes rows needs a real user id).
        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Email == "admin@scm.local");
        if (adminUser is null)
        {
            var adminRole = await db.Roles.FirstAsync(r => r.Name == RoleNames.Administrator);
            adminUser = new User
            {
                FullName       = "System Administrator",
                Email          = "admin@scm.local",
                PasswordHash   = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!", workFactor: 12),
                IsActive       = true,
                EmailConfirmed = true,
                CreatedAt      = DateTime.UtcNow
            };
            adminUser.UserRoles.Add(new UserRole { User = adminUser, Role = adminRole, AssignedAt = DateTime.UtcNow });
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
        }

        if (!await db.StatusTypes.AnyAsync(s => s.EntityType == "Supplier"))
        {
            var now = DateTime.UtcNow;
            db.StatusTypes.AddRange(
                new StatusType { EntityType = "Supplier", StatusName = "Active",   CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "Supplier", StatusName = "Inactive", CreatedBy = adminUser.Id, CreatedAt = now }
            );
            await db.SaveChangesAsync();
        }
    }
}
