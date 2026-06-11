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
            .Include(b => b.DoctorProfile)
            .FirstOrDefaultAsync(b => b.Id == request.DoctorId && b.Type == BusinessType.Doctor, ct);

        if (parent is null || parent.DoctorProfile is null)
            throw new NotFoundException("Parent doctor not found");

        if (!await db.Set<City>().AnyAsync(c => c.Id == (request.CityId ?? parent.CityId), ct))
            throw new NotFoundException("City not found");

        var branch = new Branch
        {
            ParentBusinessId = request.DoctorId,
            Name             = request.Name.Trim(),
            CityId           = request.CityId ?? parent.CityId,
            Address          = request.Address?.Trim(),
            Latitude         = request.Latitude,
            Longitude        = request.Longitude,
            VisitPrice       = request.VisitPrice,
            WorkingDays      = DoctorMappingHelpers.MapWorkingDays(request.WorkingDays),
            PhoneNumbers     = DoctorMappingHelpers.MapPhoneNumbers(request.PhoneNumbers),
        };

        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);

        return new CreateBranchResponse(branch.Id, branch.Name);
    }
}
