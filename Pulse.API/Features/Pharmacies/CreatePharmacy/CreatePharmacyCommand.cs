using MediatR;

namespace Pulse.API.Features.Pharmacies.CreatePharmacy;

using Pulse.API.Features.Shared;

public record CreatePharmacyCommand(
    string Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude,
    List<BusinessServiceItem>? Services = null
) : IRequest<PharmacyResponse>;
