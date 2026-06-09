using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Shared;

public static class PaginationHelper
{
    public static async Task<PaginatedResponse<T>> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var total = await query.CountAsync(ct);
        var safePage = Math.Max(1, page);
        var safePageSize = Math.Clamp(pageSize, 1, 50);
        var items = await query.Skip((safePage - 1) * safePageSize).Take(safePageSize).ToListAsync(ct);
        return new PaginatedResponse<T>(items, safePage, safePageSize, total, safePage * safePageSize < total);
    }
}
