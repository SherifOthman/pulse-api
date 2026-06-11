using MediatR;

namespace Pulse.API.Features.Cities.DeleteCity;

public record DeleteCityCommand(Guid Id) : IRequest;
