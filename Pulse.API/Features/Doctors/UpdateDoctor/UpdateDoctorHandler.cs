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
            .Include(b => b.Doctor)
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

        if (business.Doctor is not null)
        {
            if (request.SpecializationId.HasValue)
            {
                if (!await db.Set<Specialization>().AnyAsync(s => s.Id == request.SpecializationId.Value, ct))
                    throw new NotFoundException("Specialization not found");
                business.Doctor.SpecializationId = request.SpecializationId.Value;
            }

            if (request.VisitPrice.HasValue)   business.Doctor.VisitPrice = request.VisitPrice;
            if (request.Gender.HasValue)        business.Doctor.Gender     = request.Gender.Value;
        }

        if (request.WorkingDays is not null)
            business.WorkingDays = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays);

        if (request.PhoneNumbers is not null)
            business.PhoneNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers);

        await db.SaveChangesAsync(ct);
        return new UpdateDoctorResponse(business.Id, business.Name);
    }
}
