using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> b)
    {
        b.ToTable("PurchaseRequests");
        b.HasKey(r => r.Id);
        b.Property(r => r.EstimatedCost).HasColumnType("decimal(18,2)");

        b.HasOne(r => r.Status)
         .WithMany()
         .HasForeignKey(r => r.StatusId) // FK -> StatusTypes (EntityType=PurchaseRequest)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(r => r.RequestedByUser)
         .WithMany()
         .HasForeignKey(r => r.RequestedByUserId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        b.HasOne(r => r.ApprovedByUser)
         .WithMany()
         .HasForeignKey(r => r.ApprovedByUserId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);
    }
}
