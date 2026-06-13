using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.ToTable("DoctorProfiles");

        builder.HasKey(x => x.BusinessId);

        builder.Property(x => x.Gender)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.VisitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.HasMany(x => x.DoctorSpecializations)
            .WithOne(x => x.DoctorProfile)
            .HasForeignKey(x => x.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DoctorSpecializationConfiguration : IEntityTypeConfiguration<DoctorSpecialization>
{
    public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
    {
        builder.ToTable("DoctorSpecializations");

        builder.HasKey(x => new { x.DoctorProfileId, x.SpecializationId });

        builder.HasOne(x => x.Specialization)
            .WithMany(x => x.DoctorSpecializations)
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
