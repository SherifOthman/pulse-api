using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared;

public record BusinessServiceItem(Guid? Id, string? Name);

public static class BusinessServiceHelpers
{
    public static async Task LinkServicesAsync(
        AppDbContext db,
        Guid businessId,
        BusinessType businessType,
        List<BusinessServiceItem>? items,
        CancellationToken ct)
    {
        if (items is null || items.Count == 0) return;

        var resolvedIds = new List<Guid>();

        foreach (var item in items)
        {
            if (item.Id.HasValue)
            {
                var ok = await db.Set<Service>()
                    .AnyAsync(s => s.Id == item.Id.Value && s.BusinessType == businessType, ct);
                if (!ok) throw new NotFoundException($"Service '{item.Id}' not found");
                if (!resolvedIds.Contains(item.Id.Value))
                    resolvedIds.Add(item.Id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var trimmed = item.Name.Trim();
                var existing = await db.Set<Service>()
                    .FirstOrDefaultAsync(s => s.BusinessType == businessType && s.Name == trimmed, ct);

                if (existing is not null)
                {
                    if (!resolvedIds.Contains(existing.Id))
                        resolvedIds.Add(existing.Id);
                }
                else
                {
                    var svc = new Service { Name = trimmed, BusinessType = businessType };
                    db.Set<Service>().Add(svc);
                    await db.SaveChangesAsync(ct);
                    resolvedIds.Add(svc.Id);
                }
            }
        }

        foreach (var sid in resolvedIds)
            db.Set<BusinessService>().Add(new BusinessService { BusinessId = businessId, ServiceId = sid });

        await db.SaveChangesAsync(ct);
    }
}
