using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Infrastructure;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Laboratories.GetLaboratories;

public class GetLaboratoriesHandler(AppDbContext db)
    : IRequestHandler<GetLaboratoriesQuery, PaginatedResponse<LaboratoryListResponse>>
{
    public async Task<PaginatedResponse<LaboratoryListResponse>> Handle(GetLaboratoriesQuery request, CancellationToken ct)
    {
        var today = DateTime.Now.DayOfWeek;
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var bq = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Laboratory && b.ParentBusinessId == null)
            .ApplyFilters(bq);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            b.ProfileImageUrl,
            GovernorateName = b.City.Governorate.Name,
            AvgRating = b.Testimonials
                .Select(t => (double)t.Rating)
                .DefaultIfEmpty()
                .Average(),
            TotalRatings = b.Testimonials.Count,
            NextWorkingDay = b.WorkingDays
                .Select(w => new
                {
                    w.Day,
                    w.StartTime,
                    w.EndTime,
                    DaysUntil = w.Day >= today ? w.Day - today : 7 - (int)today + (int)w.Day
                })
                .OrderBy(w => w.DaysUntil)
                .FirstOrDefault(),
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
            var isOpen = r.NextWorkingDay != null
                && r.NextWorkingDay.Day == today
                && r.NextWorkingDay.StartTime <= now
                && r.NextWorkingDay.EndTime >= now;

            return new LaboratoryListResponse(
                r.Id,
                r.Name,
                r.ProfileImageUrl,
                r.GovernorateName,
                Math.Round(r.AvgRating, 1),
                r.TotalRatings,
                r.NextWorkingDay != null ? (int)r.NextWorkingDay.Day : 0,
                r.NextWorkingDay?.StartTime.ToString("HH:mm"),
                r.NextWorkingDay?.EndTime.ToString("HH:mm"),
                isOpen,
                r.CreatedBy
            );
        }).ToList();

        return new PaginatedResponse<LaboratoryListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
