using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> b)
    {
        b.ToTable("StockTransactions");
        b.HasKey(t => t.Id);
        b.Property(t => t.TransactionType).IsRequired().HasMaxLength(30);
        b.Property(t => t.ReferenceType).HasMaxLength(30);

        b.HasOne(t => t.Product)
         .WithMany()
         .HasForeignKey(t => t.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(t => t.Warehouse)
         .WithMany()
         .HasForeignKey(t => t.WarehouseId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
