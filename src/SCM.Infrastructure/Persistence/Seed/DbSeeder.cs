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

        // Week 4 - Procurement module status types
        if (!await db.StatusTypes.AnyAsync(s => s.EntityType == "PurchaseRequest"))
        {
            var now = DateTime.UtcNow;
            db.StatusTypes.AddRange(
                new StatusType { EntityType = "PurchaseRequest", StatusName = "Pending",      CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseRequest", StatusName = "UnderReview",  CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseRequest", StatusName = "Approved",     CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseRequest", StatusName = "Rejected",     CreatedBy = adminUser.Id, CreatedAt = now }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.StatusTypes.AnyAsync(s => s.EntityType == "PurchaseOrder"))
        {
            var now = DateTime.UtcNow;
            db.StatusTypes.AddRange(
                new StatusType { EntityType = "PurchaseOrder", StatusName = "Draft",              CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseOrder", StatusName = "Sent",                CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseOrder", StatusName = "PartiallyDelivered",  CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseOrder", StatusName = "Delivered",           CreatedBy = adminUser.Id, CreatedAt = now },
                new StatusType { EntityType = "PurchaseOrder", StatusName = "Cancelled",           CreatedBy = adminUser.Id, CreatedAt = now }
            );
            await db.SaveChangesAsync();
        }

        // Week 4 - sample products so the demo has real data to work with
        if (!await db.Products.AnyAsync())
        {
            var now = DateTime.UtcNow;
            db.Products.AddRange(
                new Product { SKU = "WHT-FLR-25KG", Name = "Wheat Flour (25kg bag)",       Category = "Raw Material", UnitPrice = 850.00m,  ReorderThreshold = 50,  IsActive = true, CreatedAt = now },
                new Product { SKU = "SUG-WHT-50KG", Name = "White Sugar (50kg sack)",       Category = "Raw Material", UnitPrice = 2100.00m, ReorderThreshold = 30,  IsActive = true, CreatedAt = now },
                new Product { SKU = "COOK-OIL-20L",  Name = "Cooking Oil (20L container)",   Category = "Raw Material", UnitPrice = 1450.00m, ReorderThreshold = 40,  IsActive = true, CreatedAt = now },
                new Product { SKU = "PKG-BOX-STD",   Name = "Standard Packaging Box",        Category = "Packaging",   UnitPrice = 25.00m,   ReorderThreshold = 500, IsActive = true, CreatedAt = now },
                new Product { SKU = "LBL-PRD-ROLL",  Name = "Product Label Roll (1000 pcs)", Category = "Packaging",   UnitPrice = 320.00m,  ReorderThreshold = 100, IsActive = true, CreatedAt = now }
            );
            await db.SaveChangesAsync();
        }
    }
}
