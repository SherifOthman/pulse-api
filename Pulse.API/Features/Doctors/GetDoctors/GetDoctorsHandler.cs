using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Doctors.GetDoctors;

public class GetDoctorsHandler(AppDbContext db) : IRequestHandler<GetDoctorsQuery, PaginatedResponse<DoctorListResponse>>
{
    public async Task<PaginatedResponse<DoctorListResponse>> Handle(GetDoctorsQuery request, CancellationToken ct)
    {
        var today = DateTime.Now.DayOfWeek;
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var bq = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking() 
            .Where(b => b.Type == BusinessType.Doctor && b.ParentBusinessId == null)
            .ApplyFilters(bq);

        if (request.Gender.HasValue)
            query = query.Where(b => b.Doctor!.Gender == request.Gender.Value);

        if (request.SpecializationId.HasValue)
            query = query.Where(b => b.Doctor!.SpecializationId == request.SpecializationId.Value);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            SpecializationName = b.Doctor!.Specialization.Name,
            b.ProfileImageUrl,
            b.Doctor.VisitPrice,
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
            "price" => desc
                ? projected.OrderByDescending(x => x.VisitPrice).ThenBy(x => x.Id)
                : projected.OrderBy(x => x.VisitPrice).ThenBy(x => x.Id),
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

            return new DoctorListResponse(
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
                // Doctor-specific
                r.SpecializationName,
                r.VisitPrice,
                r.CreatedBy
            );
        }).ToList();

        return new PaginatedResponse<DoctorListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
