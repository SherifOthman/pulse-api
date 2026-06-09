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
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.WorkingHours)
            .HasMaxLength(100)
            .IsRequired(false); ;

        builder.Property(x => x.Address)
            .HasMaxLength(250)
            .IsRequired(false); ;

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(500)
            .IsRequired(false); ;

        builder.Property(x => x.CoverImageUrl)
            .HasMaxLength(500)
            .IsRequired(false); ;


        builder.HasOne(x => x.ParentBusiness)
            .WithMany(x => x.Branches)
            .HasForeignKey(x => x.ParentBusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.City)
            .WithMany(x=>x.Businesses)
            .HasForeignKey(x=>x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}
