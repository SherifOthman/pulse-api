using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
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
            .FirstOrDefaultAsync(b =>
                b.Id == request.DoctorId &&
                b.Type == BusinessType.Doctor &&
                b.ParentBusinessId == null, ct);

        if (parent is null || parent.Doctor is null)
            throw new NotFoundException("Parent doctor not found");

        var branch = new Business
        {
            Name             = request.Name.Trim(),
            Type             = BusinessType.Doctor,
            CityId           = request.CityId ?? parent.CityId,
            Address          = request.Address?.Trim(),
            Latitude         = request.Latitude,
            Longitude        = request.Longitude,
            ParentBusinessId = request.DoctorId,
            CreatedByUserId  = currentUser.Id,
            Doctor = new Doctor
            {
                SpecializationId = parent.Doctor.SpecializationId,
                VisitPrice       = request.VisitPrice,
                Gender           = parent.Doctor.Gender,
            },
            WorkingDays  = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays),
            PhoneNumbers = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers),
        };

        db.Businesses.Add(branch);
        await db.SaveChangesAsync(ct);

        return new CreateBranchResponse(branch.Id, branch.Name);
    }
}
