using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Doctors.Branches.CreateBranch;

public class CreateBranchHandler(AppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<CreateBranchCommand, CreateBranchResponse>
{
    public async Task<CreateBranchResponse> Handle(CreateBranchCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Branch name is required");

        var parent = await db.Businesses
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.Id == request.DoctorId && b.Type == BusinessType.Doctor && b.ParentBusinessId == null, ct);

        if (parent is null || parent.Doctor is null)
            throw new NotFoundException("Parent doctor not found");

        var branch = new Business
        {
            Name = request.Name.Trim(),
            Type = BusinessType.Doctor,
            CityId = request.CityId ?? parent.CityId,
            Address = request.Address?.Trim(),
            Description = request.Description?.Trim(),
            ProfileImageUrl = request.ProfileImageUrl?.Trim(),
            CoverImageUrl = request.CoverImageUrl?.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ParentBusinessId = request.DoctorId,
            CreatedByUserId = currentUser.Id,
        };

        db.Businesses.Add(branch);

        db.Set<Doctor>().Add(new Doctor
        {
            BusinessId = branch.Id,
            SpecializationId = parent.Doctor.SpecializationId,
            VisitPrice = request.VisitPrice,
            Gender = parent.Doctor.Gender,
        });

        if (request.WorkingDays is not null)
        {
            foreach (var wd in request.WorkingDays)
            {
                db.Set<WorkingDay>().Add(new WorkingDay
                {
                    BusinessId = branch.Id,
                    Day = (System.DayOfWeek)wd.Day,
                    StartTime = TimeOnly.Parse(wd.StartTime),
                    EndTime = TimeOnly.Parse(wd.EndTime),
                });
            }
        }

        if (request.PhoneNumbers is not null)
        {
            foreach (var pn in request.PhoneNumbers)
            {
                db.Set<PhoneNumber>().Add(new PhoneNumber
                {
                    BusinessId = branch.Id,
                    Number = pn.Number,
                    Type = pn.Type,
                });
            }
        }

        await db.SaveChangesAsync(ct);

        return new CreateBranchResponse(branch.Id, branch.Name);
    }
}
