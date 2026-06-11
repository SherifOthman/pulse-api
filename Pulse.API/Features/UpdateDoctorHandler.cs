using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.UpdateDoctor;

public class UpdateDoctorHandler(AppDbContext db)
    : IRequestHandler<UpdateDoctorCommand, UpdateDoctorResponse>
{
    public async Task<UpdateDoctorResponse> Handle(UpdateDoctorCommand request, CancellationToken ct)
    {
        var business = await db.Businesses
            .Include(b => b.DoctorProfile)
            .Include(b => b.WorkingDays)
            .Include(b => b.PhoneNumbers)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.Type == BusinessType.Doctor, ct);

        if (business is null)
            throw new NotFoundException("Doctor not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            business.Name = request.Name.Trim();

        if (request.CityId.HasValue)
        {
            if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId.Value, ct))
                throw new NotFoundException("City not found");
            business.CityId = request.CityId.Value;
        }

        if (request.Address is not null)
            business.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        if (request.Description is not null)
            business.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        if (request.ProfileImageUrl is not null)
            business.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();

        if (request.CoverImageUrl is not null)
            business.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim();

        if (request.Latitude.HasValue)  business.Latitude  = request.Latitude;
        if (request.Longitude.HasValue) business.Longitude = request.Longitude;

        if (business.DoctorProfile is not null)
        {
            if (request.SpecializationId.HasValue)
            {
                if (!await db.Set<Specialization>().AnyAsync(s => s.Id == request.SpecializationId.Value, ct))
                    throw new NotFoundException("Specialization not found");
                business.DoctorProfile.SpecializationId = request.SpecializationId.Value;
            }

            if (request.Gender.HasValue) business.DoctorProfile.Gender = request.Gender.Value;
        }

        if (request.WorkingDays is not null)
        {
            // Delete existing rows and insert fresh ones.
            // We add new entities directly to the DbSet (not via the nav collection)
            // to avoid EF re-evaluating the relationship when the nav collection changes.
            db.Set<WorkingDay>().RemoveRange(business.WorkingDays);
            var newDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);
            foreach (var day in newDays)
            {
                day.BusinessId = business.Id;
                db.Set<WorkingDay>().Add(day);
            }
        }

        if (request.PhoneNumbers is not null)
        {
            db.Set<PhoneNumber>().RemoveRange(business.PhoneNumbers);
            var newNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);
            foreach (var phone in newNumbers)
            {
                phone.BusinessId = business.Id;
                db.Set<PhoneNumber>().Add(phone);
            }
        }

        await db.SaveChangesAsync(ct);
        return new UpdateDoctorResponse(business.Id, business.Name);
    }
}
