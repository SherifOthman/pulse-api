using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class PharmacyProfileConfiguration : IEntityTypeConfiguration<PharmacyProfile>
{
    public void Configure(EntityTypeBuilder<PharmacyProfile> builder)
    {
        builder.ToTable("PharmacyProfiles");

        builder.HasKey(x => x.BusinessId);

        builder.Property(x => x.LicenseNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.Is24Hours)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.HasDelivery)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.HasInventory)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
