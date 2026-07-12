using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class PurchaseRequestApprovalConfiguration : IEntityTypeConfiguration<PurchaseRequestApproval>
{
    public void Configure(EntityTypeBuilder<PurchaseRequestApproval> b)
    {
        b.ToTable("PurchaseRequestApprovals");
        b.HasKey(a => a.Id);
        b.Property(a => a.ApproverRole).IsRequired().HasMaxLength(50);
        b.Property(a => a.Decision).IsRequired().HasMaxLength(30);

        b.HasOne(a => a.PurchaseRequest)
         .WithMany(r => r.Approvals)
         .HasForeignKey(a => a.PurchaseRequestId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(a => a.ApproverUser)
         .WithMany()
         .HasForeignKey(a => a.ApproverUserId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);
    }
}
