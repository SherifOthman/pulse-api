using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class LaboratoryProfileConfiguration : IEntityTypeConfiguration<LaboratoryProfile>
{
    public void Configure(EntityTypeBuilder<LaboratoryProfile> builder)
    {
        builder.ToTable("LaboratoryProfiles");

        builder.HasKey(x => x.BusinessId);

        builder.Property(x => x.AccreditationNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.OffersHomeVisit)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.TestCatalogUrl)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}
