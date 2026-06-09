using MediatR;

namespace Pulse.API.Features.Cities.GetCities;

public record GetCitiesQuery(Guid? GovernorateId, int? BusinessType) : IRequest<List<CityResponse>>;
