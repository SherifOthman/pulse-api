using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class WorkingDayConfiguration : IEntityTypeConfiguration<WorkingDay>
{
    public void Configure(EntityTypeBuilder<WorkingDay> builder)
    {
        builder.ToTable("WorkingDays");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Day)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnType("time");

        builder.Property(x => x.EndTime)
            .HasColumnType("time");

        // FK to Business (nullable — either BusinessId or BranchId is set, not both)
        builder.HasOne(x => x.Business)
            .WithMany(x => x.WorkingDays)
            .HasForeignKey(x => x.BusinessId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // FK to Branch (nullable)
        builder.HasOne(x => x.Branch)
            .WithMany(x => x.WorkingDays)
            .HasForeignKey(x => x.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction); // Cascade already fires via Business → Branch

        builder.HasIndex(x => x.BusinessId);
        builder.HasIndex(x => x.BranchId);
    }
}
