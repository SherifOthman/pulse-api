using MediatR;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

public class CreateLaboratoryHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateLaboratoryCommand, LabResponse>
{
    public async Task<LabResponse> Handle(CreateLaboratoryCommand request, CancellationToken ct)
    {
        var business = await BusinessMappingHelpers.CreateBusinessAsync(
            db, request.Name, request.CityId, request.Address,
            request.Description, request.ProfileImageUrl, request.CoverImageUrl,
            request.Latitude, request.Longitude,
            BusinessType.Laboratory, currentUser.Id, ct);

        business.LaboratoryProfile = new LaboratoryProfile();
        business.WorkingDays       = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);
        business.PhoneNumbers      = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        if (request.Services is { Count: > 0 })
            await BusinessServiceHelpers.LinkServicesAsync(
                db, business.Id, BusinessType.Laboratory, request.Services, ct);

        return new LabResponse(business.Id, business.Name);
    }
}
