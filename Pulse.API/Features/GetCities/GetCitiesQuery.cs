using MediatR;

namespace Pulse.API.Features.Cities.GetCities;

public record GetCitiesQuery(Guid? GovernorateId, int? BusinessType, bool All = false) : IRequest<List<CityResponse>>;
