using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> b)
    {
        b.ToTable("Warehouses");
        b.HasKey(w => w.Id);
        b.Property(w => w.Name).IsRequired().HasMaxLength(200);
    }
}
