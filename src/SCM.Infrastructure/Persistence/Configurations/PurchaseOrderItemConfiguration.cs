using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> b)
    {
        b.ToTable("PurchaseOrderItems");
        b.HasKey(i => i.Id);
        b.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");

        b.HasOne(i => i.PurchaseOrder)
         .WithMany(o => o.Items)
         .HasForeignKey(i => i.PurchaseOrderId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(i => i.Product)
         .WithMany(p => p.PurchaseOrderItems)
         .HasForeignKey(i => i.ProductId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
