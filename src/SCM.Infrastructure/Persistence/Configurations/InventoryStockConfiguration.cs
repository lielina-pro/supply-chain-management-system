using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> b)
    {
        b.ToTable("InventoryStock");
        b.HasKey(s => s.Id);
        b.HasIndex(s => new { s.ProductId, s.WarehouseId }).IsUnique();

        b.HasOne(s => s.Product)
         .WithMany()
         .HasForeignKey(s => s.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(s => s.Warehouse)
         .WithMany()
         .HasForeignKey(s => s.WarehouseId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
