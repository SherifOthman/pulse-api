using Pulse.API.Infrastructure;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Doctors.GetMobileDoctors;

public class GetMobileDoctorsHandler(AppDbContext db)
    : IRequestHandler<GetMobileDoctorsQuery, PaginatedResponse<DoctorMobileListResponse>>
{
    public async Task<PaginatedResponse<DoctorMobileListResponse>> Handle(
        GetMobileDoctorsQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);
        var bq    = request.BusinessQuery;

        var query = db.Businesses
            .AsNoTracking()
            .Where(b => b.Type == BusinessType.Doctor)
            .ApplyFilters(bq);

        if (request.Gender.HasValue)
            query = query.Where(b => b.DoctorProfile!.Gender == request.Gender.Value);

        if (request.SpecializationId.HasValue)
            query = query.Where(b => b.DoctorProfile!.SpecializationId == request.SpecializationId.Value);

        var projected = query.Select(b => new
        {
            b.Id,
            b.Name,
            b.ProfileImageUrl,
            SpecializationName = b.DoctorProfile!.Specialization.Name,
            GovernorateName    = b.City.Governorate.Name,
            AvgRating          = b.Testimonials.Select(t => (double)t.Rating).DefaultIfEmpty().Average(),
            TotalRatings       = b.Testimonials.Count,
            NextWorkingDay     = b.WorkingDays
                .Select(w => new
                {
                    w.Day, w.StartTime, w.EndTime,
                    DaysUntil = w.Day >= today ? w.Day - today : 7 - (int)today + (int)w.Day,
                })
                .OrderBy(w => w.DaysUntil)
                .FirstOrDefault(),
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

        var items = raw.Items.Select(r =>
        {
            var isOpen = r.NextWorkingDay is not null
                && r.NextWorkingDay.Day == today
                && r.NextWorkingDay.StartTime <= now
                && r.NextWorkingDay.EndTime   >= now;

            return new DoctorMobileListResponse(
                r.Id, r.Name, UrlHelper.ToAbsolute(r.ProfileImageUrl, request.BaseUrl),
                r.GovernorateName,
                Math.Round(r.AvgRating, 1), r.TotalRatings,
                r.NextWorkingDay is not null ? (int)r.NextWorkingDay.Day : 0,
                r.NextWorkingDay?.StartTime.ToString("HH:mm"),
                r.NextWorkingDay?.EndTime.ToString("HH:mm"),
                isOpen, r.SpecializationName
            );
        }).ToList();

        return new PaginatedResponse<DoctorMobileListResponse>(
            items, raw.Page, raw.PageSize, raw.TotalCount, raw.HasMore);
    }
}
