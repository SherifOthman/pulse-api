namespace Pulse.API.Features.Cities.GetCities;

public record GetCitiesResponse(
    Guid Id,
    Guid GovernorateId,
    string Name
);
