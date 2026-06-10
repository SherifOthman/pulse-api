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

        if (request.CityId.HasValue && !await db.Set<City>().AnyAsync(c => c.Id == request.CityId.Value, ct))
            throw new NotFoundException("City not found");

        if (request.SpecializationId.HasValue
            && !await db.Set<Specialization>().AnyAsync(s => s.Id == request.SpecializationId.Value, ct))
            throw new NotFoundException("Specialization not found");

        if (request.WorkingDays is not null)
        {
            foreach (var wd in request.WorkingDays)
            {
                if (!TimeOnly.TryParse(wd.StartTime, out var start) || !TimeOnly.TryParse(wd.EndTime, out var end))
                    throw new BadRequestException($"Invalid time format for day {wd.Day}");
                if (start >= end)
                    throw new BadRequestException($"Start time must be before end time for day {wd.Day}");
            }
        }

        var business = new Business
        {
            Name = request.Name.Trim(),
            Type = BusinessType.Doctor,
            CityId = request.CityId ?? Guid.Empty,
            Address = request.Address?.Trim(),
            Description = request.Description?.Trim(),
            ProfileImageUrl = request.ProfileImageUrl?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            CreatedByUserId = currentUser.Id,
            Doctor = new Doctor
            {
                SpecializationId = request.SpecializationId ?? Guid.Empty,
                VisitPrice = request.VisitPrice,
                Gender = request.Gender ?? Gender.Male,
            },
            WorkingDays = request.WorkingDays?.Select(wd => new WorkingDay
            {
                Day = (System.DayOfWeek)wd.Day,
                StartTime = TimeOnly.Parse(wd.StartTime),
                EndTime = TimeOnly.Parse(wd.EndTime),
            }).ToList() ?? [],
            PhoneNumbers = request.PhoneNumbers?.Select(pn => new PhoneNumber
            {
                Number = pn.Number,
                Type = pn.Type,
            }).ToList() ?? [],
        };

        db.Businesses.Add(business);
        await db.SaveChangesAsync(ct);

        return new CreateDoctorResponse(business.Id, business.Name);
    }
}
