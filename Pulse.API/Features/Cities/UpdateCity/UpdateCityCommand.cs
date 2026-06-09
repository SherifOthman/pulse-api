using MediatR;

namespace Pulse.API.Features.Cities.UpdateCity;

public record UpdateCityCommand(Guid Id, string? Name, Guid? GovernorateId) : IRequest<CityResponse>;
