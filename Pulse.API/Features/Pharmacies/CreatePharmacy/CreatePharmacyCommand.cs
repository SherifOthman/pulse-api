using MediatR;

namespace Pulse.API.Features.Pharmacies.CreatePharmacy;

public record CreatePharmacyCommand(
    string Name,
    Guid CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl
) : IRequest<PharmacyResponse>;
