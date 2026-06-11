using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Doctors.Services.GetDoctorServices;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Services.GetAvailableServices;

public class GetAvailableServicesHandler(AppDbContext db)
    : IRequestHandler<GetAvailableServicesQuery, List<DoctorServiceResponse>>
{
    public async Task<List<DoctorServiceResponse>> Handle(
        GetAvailableServicesQuery request, CancellationToken ct)
    {
        return await db.Set<Domain.Entities.Service>()
            .AsNoTracking()
            .Where(s => s.BusinessType == BusinessType.Doctor)
            .OrderBy(s => s.Name)
            .Select(s => new DoctorServiceResponse(s.Id, s.Name))
            .ToListAsync(ct);
    }
}
