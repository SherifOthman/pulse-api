using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Generic services management endpoints shared across all business types.
/// Registered for Pharmacy, Lab, and Radiology using their own route segments.
/// Doctor services use dedicated endpoints under Features/Doctors/Services/.
/// </summary>
public record BusinessServiceResponse(Guid Id, string Name);
public record UpdateBusinessServicesRequest(List<BusinessServiceItem> Services);

public static class BusinessServicesEndpoints
{
    /// <summary>
    /// Registers GET available, GET linked, and PUT update endpoints
    /// for a given business type at /dashboard/{segment}/.
    /// </summary>
    public static void MapForBusinessType(
        IEndpointRouteBuilder app,
        string segment,
        BusinessType businessType)
    {
        // GET /dashboard/{segment}/services/available
        app.MapGet($"/dashboard/{segment}/services/available",
            async (AppDbContext db, CancellationToken ct) =>
            {
                var result = await db.Set<Service>()
                    .AsNoTracking()
                    .Where(s => s.BusinessType == businessType)
                    .OrderBy(s => s.Name)
                    .Select(s => new BusinessServiceResponse(s.Id, s.Name))
                    .ToListAsync(ct);
                return Results.Ok(result);
            }).RequireAuthorization("ManagerOrAdmin");

        // GET /dashboard/{segment}/{id}/services
        app.MapGet($"/dashboard/{segment}/{{id:guid}}/services",
            async (Guid id, AppDbContext db, CancellationToken ct) =>
            {
                var exists = await db.Businesses.AnyAsync(
                    b => b.Id == id && b.Type == businessType && b.ParentBusinessId == null, ct);
                if (!exists) return Results.NotFound();

                var result = await db.Set<BusinessService>()
                    .AsNoTracking()
                    .Where(bs => bs.BusinessId == id)
                    .OrderBy(bs => bs.Service.Name)
                    .Select(bs => new BusinessServiceResponse(bs.Service.Id, bs.Service.Name))
                    .ToListAsync(ct);
                return Results.Ok(result);
            }).RequireAuthorization("ManagerOrAdmin");

        // PUT /dashboard/{segment}/{id}/services
        app.MapPut($"/dashboard/{segment}/{{id:guid}}/services",
            async (Guid id, UpdateBusinessServicesRequest body, AppDbContext db, CancellationToken ct) =>
            {
                var business = await db.Businesses
                    .Include(b => b.BusinessServices)
                    .FirstOrDefaultAsync(
                        b => b.Id == id && b.Type == businessType && b.ParentBusinessId == null, ct);
                if (business is null) return Results.NotFound();

                var resolvedIds = new List<Guid>();

                foreach (var item in body.Services)
                {
                    if (item.Id.HasValue)
                    {
                        var ok = await db.Set<Service>()
                            .AnyAsync(s => s.Id == item.Id.Value && s.BusinessType == businessType, ct);
                        if (!ok) return Results.NotFound(new { error = $"Service '{item.Id}' not found" });
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

                // Sync links
                var toRemove = business.BusinessServices
                    .Where(bs => !resolvedIds.Contains(bs.ServiceId)).ToList();
                db.Set<BusinessService>().RemoveRange(toRemove);

                var existingIds = business.BusinessServices.Select(bs => bs.ServiceId).ToHashSet();
                foreach (var sid in resolvedIds.Where(sid => !existingIds.Contains(sid)))
                    db.Set<BusinessService>().Add(new BusinessService { BusinessId = id, ServiceId = sid });

                await db.SaveChangesAsync(ct);

                var updated = await db.Set<Service>()
                    .AsNoTracking()
                    .Where(s => resolvedIds.Contains(s.Id))
                    .OrderBy(s => s.Name)
                    .Select(s => new BusinessServiceResponse(s.Id, s.Name))
                    .ToListAsync(ct);

                return Results.Ok(new { services = updated });
            }).RequireAuthorization("ManagerOrAdmin");
    }
}
