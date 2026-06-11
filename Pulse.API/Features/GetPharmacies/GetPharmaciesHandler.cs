using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Pharmacies.GetPharmacies;

public class GetPharmaciesHandler(AppDbContext db)
    : IRequestHandler<GetPharmaciesQuery, PaginatedResponse<PharmacyListResponse>>
{
    public async Task<PaginatedResponse<PharmacyListResponse>> Handle(GetPharmaciesQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);
        var bq    = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Pharmacy)
            .ApplyFilters(bq);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            b.ProfileImageUrl,
            GovernorateName = b.City.Governorate.Name,
            AvgRating       = b.Testimonials.Select(t => (double)t.Rating).DefaultIfEmpty().Average(),
            CreatedBy       = b.CreatedByUser != null ? b.CreatedByUser.FullName : null,
            WorkingDays     = b.WorkingDays
                .Select(w => new { w.Day, w.StartTime, w.EndTime })
                .ToList(),
        });

        var desc = bq.SortDirection?.ToLower() == "desc";
        projected = bq.SortBy?.ToLower() switch
        {
            "rating" => desc ? projected.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id)
                              : projected.OrderBy(x => x.AvgRating).ThenBy(x => x.Id),
            _        => desc ? projected.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                              : projected.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        var raw = await projected.ToPaginatedAsync(bq.Page, bq.PageSize, ct);

        // Compute IsOpen in memory after fetching — avoids EF Core DateTime.Now translation issues
        var items = raw.Items.Select(r =>
        {
            var todayRecord = r.WorkingDays.FirstOrDefault(w => w.Day == today);
            var isOpen = todayRecord is not null
                && todayRecord.StartTime <= now
                && todayRecord.EndTime   >= now;

            return new PharmacyListResponse(
                r.Id, r.Name, r.ProfileImageUrl,
                r.GovernorateName,
                Math.Round(r.AvgRating, 1),
                isOpen,
                r.CreatedBy
            );
        }).ToList();

        return new PaginatedResponse<PharmacyListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
