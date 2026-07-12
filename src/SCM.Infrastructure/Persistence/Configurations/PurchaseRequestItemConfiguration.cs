using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class PurchaseRequestItemConfiguration : IEntityTypeConfiguration<PurchaseRequestItem>
{
    public void Configure(EntityTypeBuilder<PurchaseRequestItem> b)
    {
        b.ToTable("PurchaseRequestItems");
        b.HasKey(i => i.Id);

        b.HasOne(i => i.PurchaseRequest)
         .WithMany(r => r.Items)
         .HasForeignKey(i => i.PurchaseRequestId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(i => i.Product)
         .WithMany(p => p.PurchaseRequestItems)
         .HasForeignKey(i => i.ProductId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
