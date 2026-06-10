using MediatR;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors.Branches.UpdateBranch;

public record UpdateBranchCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    decimal? VisitPrice,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
) : IRequest<UpdateBranchResponse>;
