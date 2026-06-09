using Pulse.API.Features.Cities;
using MediatR;

namespace Pulse.API.Features.Governorates.GetGovernorateCities;

public record GetGovernorateCitiesQuery(Guid Id) : IRequest<List<CityResponse>>;
