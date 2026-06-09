using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {

        builder.ToTable("Cities");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.GovernorateId);

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Governorate)
            .WithMany(x=>x.Cities)
            .HasForeignKey(x => x.GovernorateId);
    }
}
