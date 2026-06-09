namespace Pulse.API.Features.Shared;

public record BusinessQuery(
    Guid? GovernorateId,
    Guid? CityId,
    string? Name,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 10
);
