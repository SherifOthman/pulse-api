using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(x => x.BusinessId);

        builder.HasOne(x => x.Business)
            .WithOne(x => x.Doctor)
            .HasForeignKey<Doctor>(x=>x.BusinessId);

        builder.HasOne(x=>x.Specialization)
            .WithMany(x=>x.Doctors)
            .HasForeignKey(x=>x.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.VisitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Gender)
            .HasConversion<int>()
            .IsRequired();
    }
}
