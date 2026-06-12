using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.CreateDoctor;

public class CreateDoctorHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateDoctorCommand, CreateDoctorResponse>
{
    public async Task<CreateDoctorResponse> Handle(CreateDoctorCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name is required");

        if (!request.CityId.HasValue)
            throw new BadRequestException("City is required");

        if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId.Value, ct))
            throw new NotFoundException("City not found");

        if (request.SpecializationId.HasValue
            && !await db.Set<Specialization>().AnyAsync(s => s.Id == request.SpecializationId.Value, ct))
            throw new NotFoundException("Specialization not found");

        var workingDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);

        var business = new Business
        {
            Name            = request.Name.Trim(),
            Type            = BusinessType.Doctor,
            CityId          = request.CityId.Value,
            Address         = request.Address?.Trim(),
            Description     = request.Description?.Trim(),
            ProfileImageUrl = request.ProfileImageUrl?.Trim(),
            CoverImageUrl   = request.CoverImageUrl?.Trim(),
            Latitude        = request.Latitude,
            Longitude       = request.Longitude,
            CreatedByUserId = currentUser.Id,
            DoctorProfile = new DoctorProfile
            {
                SpecializationId = request.SpecializationId ?? Guid.Empty,
                Gender           = request.Gender ?? Gender.Male,
                VisitPrice       = request.VisitPrice,
            },
            WorkingDays  = workingDays,
            PhoneNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers),
        };

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        return new CreateDoctorResponse(business.Id, business.Name);
    }
}
