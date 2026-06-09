using MediatR;

namespace Pulse.API.Features.Cities.CreateCity;

public record CreateCityCommand(Guid GovernorateId, string Name) : IRequest<CityResponse>;
