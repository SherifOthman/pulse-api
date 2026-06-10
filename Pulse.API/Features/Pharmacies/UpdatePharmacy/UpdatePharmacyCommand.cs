using MediatR;

namespace Pulse.API.Features.Pharmacies.UpdatePharmacy;

public record UpdatePharmacyCommand(
    Guid Id,
    string? Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    double? Latitude,
    double? Longitude
) : IRequest<PharmacyResponse>;
