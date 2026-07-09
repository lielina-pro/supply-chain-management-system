using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCM.Domain.Entities;

namespace SCM.Infrastructure.Persistence.Configurations;

public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
{
    public void Configure(EntityTypeBuilder<StatusHistory> b)
    {
        b.ToTable("StatusHistory");
        b.HasKey(s => s.Id);
        b.Property(s => s.EntityType).IsRequired().HasMaxLength(100);
        // EntityType/EntityId is a soft link per the ER (not a real FK) - enforced in the service layer.
        b.HasOne<StatusType>()
         .WithMany()
         .HasForeignKey(s => s.ToStatusId)
         .OnDelete(DeleteBehavior.Restrict);
        b.HasOne<StatusType>()
         .WithMany()
         .HasForeignKey(s => s.FromStatusId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
