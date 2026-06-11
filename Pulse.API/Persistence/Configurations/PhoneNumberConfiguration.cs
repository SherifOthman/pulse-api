using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        builder.ToTable("PhoneNumbers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(50)
            .IsRequired(false);

        // FK to Business (nullable — either BusinessId or BranchId is set, not both)
        builder.HasOne(x => x.Business)
            .WithMany(x => x.PhoneNumbers)
            .HasForeignKey(x => x.BusinessId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // FK to Branch (nullable)
        builder.HasOne(x => x.Branch)
            .WithMany(x => x.PhoneNumbers)
            .HasForeignKey(x => x.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction); // Cascade already fires via Business → Branch

        builder.HasIndex(x => x.BusinessId);
        builder.HasIndex(x => x.BranchId);
    }
}
