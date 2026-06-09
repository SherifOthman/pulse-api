namespace Pulse.API.Features.Shared;

public record PaginatedResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    bool HasMore
);
