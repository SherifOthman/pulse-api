using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Doctors.Branches.GetBranchDetails;

public record BranchDetailsResponse(
    Guid Id,
    string Name,
    string? Address,
    string Governorate,
    Guid GovernorateId,
    string City,
    Guid CityId,
    decimal? VisitPrice,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers
);
