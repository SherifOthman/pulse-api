using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable("Testimonials");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.BusinessId, x.UserId }).IsUnique();

        builder.Property(x => x.Text)
             .HasMaxLength(500)
             .IsRequired();

        builder.Property(x=>x.Rating) 
            .IsRequired();

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Testimonials)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Testimonials)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(x => x.UserId);
    }
}
