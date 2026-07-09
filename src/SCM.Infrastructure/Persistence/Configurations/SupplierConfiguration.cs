using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> b)
    {
        b.ToTable("Suppliers");
        b.HasKey(s => s.Id);
        b.Property(s => s.CompanyName).IsRequired().HasMaxLength(200);
        b.Property(s => s.VerificationStatus).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
        b.Property(s => s.PerformanceRating).HasColumnType("decimal(3,2)");
        b.HasIndex(s => s.UserId).IsUnique(); // one supplier profile per user

        b.HasOne(s => s.Status)
         .WithMany()
         .HasForeignKey(s => s.StatusId) // FK -> StatusTypes (EntityType=Supplier). BR-05.
         .OnDelete(DeleteBehavior.Restrict);
    }
}
