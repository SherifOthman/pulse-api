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

        builder.HasOne(x => x.Specialization)
            .WithMany(x => x.Doctors)
            .HasForeignKey(x => x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Gender)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.VisitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
    }
}
