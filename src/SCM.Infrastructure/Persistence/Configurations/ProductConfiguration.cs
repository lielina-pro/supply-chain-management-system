using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(p => p.Id);
        b.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        b.HasIndex(p => p.SKU).IsUnique();
        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.Property(p => p.UnitPrice).HasColumnType("decimal(10,2)");
    }
}
