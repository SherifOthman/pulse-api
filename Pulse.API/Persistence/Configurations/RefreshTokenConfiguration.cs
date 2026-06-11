using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.RevokedByIp)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.CreatedByIp)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.ExpiresAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.RevokedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(x => x.ReplacedByTokenHash)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
