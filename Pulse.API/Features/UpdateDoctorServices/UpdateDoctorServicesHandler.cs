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
                b.Type == BusinessType.Doctor, ct);

        if (doctor is null)
            throw new NotFoundException("Doctor not found");

        var resolvedIds = new List<Guid>();
        var newServices = new List<Service>();

        foreach (var item in request.Services)
        {
            if (item.Id.HasValue)
            {
                var exists = await db.Set<Service>()
                    .AnyAsync(s => s.Id == item.Id.Value && s.BusinessType == BusinessType.Doctor, ct);
                if (!exists)
                    throw new NotFoundException($"Service with id '{item.Id}' not found");
                if (!resolvedIds.Contains(item.Id.Value))
                    resolvedIds.Add(item.Id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var trimmed = item.Name.Trim();
                var existing = await db.Set<Service>()
                    .FirstOrDefaultAsync(s => s.BusinessType == BusinessType.Doctor && s.Name == trimmed, ct);

                if (existing is not null)
                {
                    if (!resolvedIds.Contains(existing.Id))
                        resolvedIds.Add(existing.Id);
                }
                else
                {
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

        resolvedIds.AddRange(newServices.Select(s => s.Id));

        var toRemove = doctor.BusinessServices
            .Where(bs => !resolvedIds.Contains(bs.ServiceId)).ToList();
        db.Set<BusinessService>().RemoveRange(toRemove);

        var existingLinkedIds = doctor.BusinessServices.Select(bs => bs.ServiceId).ToHashSet();
        foreach (var id in resolvedIds.Where(id => !existingLinkedIds.Contains(id)))
            db.Set<BusinessService>().Add(new BusinessService { BusinessId = request.DoctorId, ServiceId = id });

        await db.SaveChangesAsync(ct);

        var updated = await db.Set<Service>()
            .AsNoTracking()
            .Where(s => resolvedIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => new DoctorServiceResponse(s.Id, s.Name))
            .ToListAsync(ct);

        return new UpdateDoctorServicesResponse(updated);
    }
}
