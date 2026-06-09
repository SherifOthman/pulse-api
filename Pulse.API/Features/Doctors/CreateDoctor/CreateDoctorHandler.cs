using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
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

        if (!await db.Set<City>().AnyAsync(c => c.Id == request.CityId, ct))
            throw new NotFoundException("City not found");

        var business = new Business
        {
            Name = request.Name.Trim(),
            Type = BusinessType.Doctor,
            CityId = request.CityId,
            Address = request.Address?.Trim(),
            Description = request.Description?.Trim(),
            ProfileImageUrl = request.ProfileImageUrl?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            CreatedByUserId = currentUser.Id,
        };

        db.Businesses.Add(business);

        if (request.SpecializationId.HasValue)
        {
            if (!await db.Set<Specialization>().AnyAsync(s => s.Id == request.SpecializationId.Value, ct))
                throw new NotFoundException("Specialization not found");
        }

        var doctor = new Doctor
        {
            BusinessId = business.Id,
            SpecializationId = request.SpecializationId ?? Guid.Empty,
            VisitPrice = request.VisitPrice,
            Gender = request.Gender ?? Gender.Male,
        };

        db.Set<Doctor>().Add(doctor);
        await db.SaveChangesAsync(ct);

        return new CreateDoctorResponse(business.Id, business.Name);
    }
}
