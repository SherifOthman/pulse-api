using Pulse.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pulse.API.Persistence.Configurations;

public class WorkingDayConfiguration : IEntityTypeConfiguration<WorkingDay>
{
    public void Configure(EntityTypeBuilder<WorkingDay> builder)
    {
        builder.ToTable("WorkingDays");

        builder.HasKey(x => x.Id);

        builder.HasOne(x=>x.Business)
            .WithMany(x=>x.WorkingDays)
            .HasForeignKey(x=>x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
