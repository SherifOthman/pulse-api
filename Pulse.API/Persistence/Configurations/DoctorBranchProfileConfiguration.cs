using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class DoctorBranchProfileConfiguration : IEntityTypeConfiguration<DoctorBranchProfile>
{
    public void Configure(EntityTypeBuilder<DoctorBranchProfile> builder)
    {
        builder.ToTable("DoctorBranchProfiles");
        builder.HasKey(x => x.BranchId);

        builder.Property(x => x.VisitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.HasOne(x => x.Branch)
            .WithOne(x => x.DoctorBranchProfile)
            .HasForeignKey<DoctorBranchProfile>(x => x.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
