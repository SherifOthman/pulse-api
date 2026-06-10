using Pulse.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Pharmacies.GetMobilePharmacies;

public class GetMobilePharmaciesHandler(AppDbContext db)
    : IRequestHandler<GetMobilePharmaciesQuery, PaginatedResponse<PharmacyMobileListResponse>>
{
    public async Task<PaginatedResponse<PharmacyMobileListResponse>> Handle(
        GetMobilePharmaciesQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);
        var bq    = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Pharmacy && b.ParentBusinessId == null)
            .ApplyFilters(bq);

        var projected = query.Select(b => new
        {
            b.Id, b.Name, b.ProfileImageUrl,
            GovernorateName = b.City.Governorate.Name,
            AvgRating    = b.Testimonials.Select(t => (double)t.Rating).DefaultIfEmpty().Average(),
            TotalRatings = b.Testimonials.Count,
            NextWorkingDay = b.WorkingDays
                .Select(w => new { w.Day, w.StartTime, w.EndTime,
                    DaysUntil = w.Day >= today ? w.Day - today : 7 - (int)today + (int)w.Day })
                .OrderBy(w => w.DaysUntil).FirstOrDefault(),
        });

        var desc = bq.SortDirection?.ToLower() == "desc";
        projected = bq.SortBy?.ToLower() switch
        {
            "rating" => desc ? projected.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id)
                              : projected.OrderBy(x => x.AvgRating).ThenBy(x => x.Id),
            _ => desc ? projected.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                      : projected.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        var raw = await projected.ToPaginatedAsync(bq.Page, bq.PageSize, ct);

        var items = raw.Items.Select(r =>
        {
            var isOpen = r.NextWorkingDay != null
                && r.NextWorkingDay.Day == today
                && r.NextWorkingDay.StartTime <= now
                && r.NextWorkingDay.EndTime >= now;

            return new PharmacyMobileListResponse(
                r.Id, r.Name, ToAbsolute(r.ProfileImageUrl, request.BaseUrl),
                r.GovernorateName,
                Math.Round(r.AvgRating, 1), r.TotalRatings,
                r.NextWorkingDay != null ? (int)r.NextWorkingDay.Day : 0,
                r.NextWorkingDay?.StartTime.ToString("HH:mm"),
                r.NextWorkingDay?.EndTime.ToString("HH:mm"),
                isOpen
            );
        }).ToList();

        return new PaginatedResponse<PharmacyMobileListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }

    private static string? ToAbsolute(string? path, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        if (path.StartsWith("http://") || path.StartsWith("https://")) return path;
        return $"{baseUrl}{(path.StartsWith('/') ? path : '/' + path)}";
    }
}
