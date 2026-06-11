using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Business");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.CoverImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Latitude)
            .HasColumnType("float")
            .IsRequired(false);

        builder.Property(x => x.Longitude)
            .HasColumnType("float")
            .IsRequired(false);

        builder.HasOne(x => x.City)
            .WithMany(x => x.Businesses)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // 1:1 profile relationships
        builder.HasOne(x => x.DoctorProfile)
            .WithOne(x => x.Business)
            .HasForeignKey<DoctorProfile>(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PharmacyProfile)
            .WithOne(x => x.Business)
            .HasForeignKey<PharmacyProfile>(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LaboratoryProfile)
            .WithOne(x => x.Business)
            .HasForeignKey<LaboratoryProfile>(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RadiologyProfile)
            .WithOne(x => x.Business)
            .HasForeignKey<RadiologyProfile>(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
