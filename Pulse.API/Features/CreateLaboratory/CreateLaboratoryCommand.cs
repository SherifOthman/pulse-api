using MediatR;
using Pulse.API.Features.Shared;

namespace Pulse.API.Features.Laboratories.CreateLaboratory;

public record CreateLaboratoryCommand(
    string Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays = null,
    List<PhoneNumberDto>? PhoneNumbers = null,
    List<BusinessServiceItem>? Services = null
) : IRequest<LabResponse>;
