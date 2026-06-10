using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Services.GetDoctorServices;

public class GetDoctorServicesHandler(AppDbContext db)
    : IRequestHandler<GetDoctorServicesQuery, List<DoctorServiceResponse>>
{
    public async Task<List<DoctorServiceResponse>> Handle(
        GetDoctorServicesQuery request, CancellationToken ct)
    {
        var exists = await db.Businesses
            .AnyAsync(b => b.Id == request.DoctorId
                        && b.Type == BusinessType.Doctor
                        && b.ParentBusinessId == null, ct);

        if (!exists)
            throw new NotFoundException("Doctor not found");

        return await db.Set<BusinessService>()
            .AsNoTracking()
            .Where(bs => bs.BusinessId == request.DoctorId)
            .OrderBy(bs => bs.Service.Name)          // order before projection
            .Select(bs => new DoctorServiceResponse(bs.Service.Id, bs.Service.Name))
            .ToListAsync(ct);
    }
}
