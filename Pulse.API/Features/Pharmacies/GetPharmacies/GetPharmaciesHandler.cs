using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Pharmacies.GetPharmacies;

public class GetPharmaciesHandler(AppDbContext db)
    : IRequestHandler<GetPharmaciesQuery, PaginatedResponse<PharmacyListResponse>>
{
    public async Task<PaginatedResponse<PharmacyListResponse>> Handle(GetPharmaciesQuery request, CancellationToken ct)
    {
        var bq = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Pharmacy && b.ParentBusinessId == null)
            .ApplyFilters(bq);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            GovernorateName = b.City.Governorate.Name,
            AvgRating = b.Testimonials
                .Select(t => (double)t.Rating)
                .DefaultIfEmpty()
                .Average(),
            Today = DateTime.Now.DayOfWeek,
            Now = TimeOnly.FromDateTime(DateTime.Now),
            WorkingDays = b.WorkingDays.Select(w => new { w.Day, w.StartTime, w.EndTime }).ToList(),
            CreatedBy = b.CreatedByUser != null ? b.CreatedByUser.FullName : null,
        });

        var desc = bq.SortDirection?.ToLower() == "desc";
        projected = bq.SortBy?.ToLower() switch
        {
            "rating" => desc
                ? projected.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.AvgRating).ThenBy(x => x.Id),
            _ => desc
                ? projected.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        var raw = await projected.ToPaginatedAsync(bq.Page, bq.PageSize, ct);

        var items = raw.Items.Select(r =>
        {
            var nextWorkingDay = r.WorkingDays
                .Select(w => new
                {
                    w.Day,
                    w.StartTime,
                    w.EndTime,
                    DaysUntil = w.Day >= r.Today ? w.Day - r.Today : 7 - (int)r.Today + (int)w.Day
                })
                .OrderBy(w => w.DaysUntil)
                .FirstOrDefault();

            var isOpen = nextWorkingDay != null
                && nextWorkingDay.Day == r.Today
                && nextWorkingDay.StartTime <= r.Now
                && nextWorkingDay.EndTime >= r.Now;

            return new PharmacyListResponse(
                r.Id,
                r.Name,
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
