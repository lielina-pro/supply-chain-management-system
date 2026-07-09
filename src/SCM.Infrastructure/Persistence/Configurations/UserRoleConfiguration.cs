using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.ToTable("UserRoles");
        b.HasKey(ur => new { ur.UserId, ur.RoleId }); // BR-09 composite PK, matches ER

        b.HasOne(ur => ur.User)
         .WithMany(u => u.UserRoles)
         .HasForeignKey(ur => ur.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(ur => ur.Role)
         .WithMany(r => r.UserRoles)
         .HasForeignKey(ur => ur.RoleId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
