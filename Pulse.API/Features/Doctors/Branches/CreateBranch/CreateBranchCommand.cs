using MediatR;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors.Branches.CreateBranch;

public record CreateBranchCommand(
    Guid DoctorId,
    string Name,
    Guid? CityId,
    string? Address,
    decimal? VisitPrice,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
) : IRequest<CreateBranchResponse>;
