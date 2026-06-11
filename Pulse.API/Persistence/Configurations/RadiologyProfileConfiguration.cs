using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class RadiologyProfileConfiguration : IEntityTypeConfiguration<RadiologyProfile>
{
    public void Configure(EntityTypeBuilder<RadiologyProfile> builder)
    {
        builder.ToTable("RadiologyProfiles");

        builder.HasKey(x => x.BusinessId);

        builder.Property(x => x.AccreditationNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.AvailableModalities)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.EquipmentDetails)
            .HasMaxLength(1000)
            .IsRequired(false);
    }
}
