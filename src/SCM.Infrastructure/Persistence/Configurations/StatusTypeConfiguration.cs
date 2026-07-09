using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class StatusTypeConfiguration : IEntityTypeConfiguration<StatusType>
{
    public void Configure(EntityTypeBuilder<StatusType> b)
    {
        b.ToTable("StatusTypes");
        b.HasKey(s => s.Id);
        b.Property(s => s.EntityType).IsRequired().HasMaxLength(100);
        b.Property(s => s.StatusName).IsRequired().HasMaxLength(100);
        b.HasIndex(s => new { s.EntityType, s.StatusName }).IsUnique();
    }
}
