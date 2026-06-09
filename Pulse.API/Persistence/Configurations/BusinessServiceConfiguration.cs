using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class BusinessServiceConfiguration : IEntityTypeConfiguration<BusinessService>
{
    public void Configure(EntityTypeBuilder<BusinessService> builder)
    {
        builder.ToTable("BusinessServices");

        builder.HasKey(x => new { x.BusinessId, x.ServiceId, });

        builder.HasOne(x => x.Business)
            .WithMany(x => x.BusinessServices)
            .HasForeignKey(x => x.BusinessId);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.BusinessServices)
            .HasForeignKey(x => x.ServiceId);


        builder.HasIndex(x => x.ServiceId);

    }

}
