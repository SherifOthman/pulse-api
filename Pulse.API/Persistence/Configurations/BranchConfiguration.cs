using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.Latitude)
            .HasColumnType("float")
            .IsRequired(false);

        builder.Property(x => x.Longitude)
            .HasColumnType("float")
            .IsRequired(false);

        builder.Property(x => x.VisitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.HasOne(x => x.ParentBusiness)
            .WithMany(x => x.Branches)
            .HasForeignKey(x => x.ParentBusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.City)
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ParentBusinessId);
        builder.HasIndex(x => x.CityId);
    }
}
