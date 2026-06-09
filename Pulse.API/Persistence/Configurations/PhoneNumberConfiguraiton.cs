using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class PhoneNumberConfiguraiton : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        builder.ToTable("PhoneNumbers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.HasOne(x => x.Business)
            .WithMany(x => x.PhoneNumbers)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
