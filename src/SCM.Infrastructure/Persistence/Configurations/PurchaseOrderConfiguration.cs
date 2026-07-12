using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> b)
    {
        b.ToTable("PurchaseOrders");
        b.HasKey(o => o.Id);
        b.Property(o => o.TotalAmount).HasColumnType("decimal(12,2)");

        b.HasOne(o => o.PurchaseRequest)
         .WithMany(r => r.PurchaseOrders)
         .HasForeignKey(o => o.PurchaseRequestId) // BR-02 enforced in service layer, not here
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.Supplier)
         .WithMany()
         .HasForeignKey(o => o.SupplierId) // BR-05 enforced in service layer, not here
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.Status)
         .WithMany()
         .HasForeignKey(o => o.StatusId) // FK -> StatusTypes (EntityType=PurchaseOrder)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
