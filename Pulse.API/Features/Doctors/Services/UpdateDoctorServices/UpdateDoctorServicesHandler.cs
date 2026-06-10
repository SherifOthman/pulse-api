using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Doctors.Services.GetDoctorServices;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Services.UpdateDoctorServices;

public class UpdateDoctorServicesHandler(AppDbContext db)
    : IRequestHandler<UpdateDoctorServicesCommand, UpdateDoctorServicesResponse>
{
    public async Task<UpdateDoctorServicesResponse> Handle(
        UpdateDoctorServicesCommand request, CancellationToken ct)
    {
        var doctor = await db.Businesses
            .Include(b => b.BusinessServices)
            .FirstOrDefaultAsync(b =>
                b.Id == request.DoctorId &&
                b.Type == BusinessType.Doctor &&
                b.ParentBusinessId == null, ct);

        if (doctor is null)
            throw new NotFoundException("Doctor not found");

        // ── Phase 1: resolve or create services ───────────────────────────────
        var resolvedIds = new List<Guid>();
        var newServices = new List<Service>();

        foreach (var item in request.Services)
        {
            if (item.Id.HasValue)
            {
                // Validate the existing service is a Doctor-type service
                var exists = await db.Set<Service>()
                    .AnyAsync(s => s.Id == item.Id.Value
                               && s.BusinessType == BusinessType.Doctor, ct);

                if (!exists)
                    throw new NotFoundException($"Service with id '{item.Id}' not found");

                if (!resolvedIds.Contains(item.Id.Value))
                    resolvedIds.Add(item.Id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var trimmed = item.Name.Trim();

                // Reuse if a service with this exact name already exists
                var existing = await db.Set<Service>()
                    .FirstOrDefaultAsync(s =>
                        s.BusinessType == BusinessType.Doctor &&
                        s.Name == trimmed, ct);

                if (existing is not null)
                {
                    if (!resolvedIds.Contains(existing.Id))
                        resolvedIds.Add(existing.Id);
                }
                else
                {
                    // Queue new service — add all at once before saving
                    var queued = newServices.FirstOrDefault(s => s.Name == trimmed);
                    if (queued is null)
                    {
                        var svc = new Service { Name = trimmed, BusinessType = BusinessType.Doctor };
                        newServices.Add(svc);
                        db.Set<Service>().Add(svc);
                    }
                }
            }
        }

        // Save new services in a single batch so we get their IDs
        if (newServices.Count > 0)
        {
            await db.SaveChangesAsync(ct);
            resolvedIds.AddRange(newServices.Select(s => s.Id));
        }

        // ── Phase 2: sync BusinessService links ───────────────────────────────
        var toRemove = doctor.BusinessServices
            .Where(bs => !resolvedIds.Contains(bs.ServiceId))
            .ToList();

        db.Set<BusinessService>().RemoveRange(toRemove);

        var existingLinkedIds = doctor.BusinessServices
            .Select(bs => bs.ServiceId)
            .ToHashSet();

        foreach (var id in resolvedIds.Where(id => !existingLinkedIds.Contains(id)))
        {
            db.Set<BusinessService>().Add(new BusinessService
            {
                BusinessId = request.DoctorId,
                ServiceId  = id,
            });
        }

        await db.SaveChangesAsync(ct);

        // ── Phase 3: return updated list ──────────────────────────────────────
        var updated = await db.Set<Service>()
            .AsNoTracking()
            .Where(s => resolvedIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => new DoctorServiceResponse(s.Id, s.Name))
            .ToListAsync(ct);

        return new UpdateDoctorServicesResponse(updated);
    }
}
