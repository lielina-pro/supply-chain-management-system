using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(u => u.Id);
        b.Property(u => u.FullName).HasMaxLength(200);
        b.Property(u => u.Email).IsRequired().HasMaxLength(256);
        b.HasIndex(u => u.Email).IsUnique();
        b.Property(u => u.PasswordHash).IsRequired();
        b.Property(u => u.PhoneNumber).HasMaxLength(30);

        b.HasOne(u => u.Supplier)
         .WithOne(s => s.User)
         .HasForeignKey<Supplier>(s => s.UserId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
