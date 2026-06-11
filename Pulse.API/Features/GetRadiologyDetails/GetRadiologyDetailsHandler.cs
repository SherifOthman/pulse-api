using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Radiology.GetRadiologyDetails;

public class GetRadiologyDetailsHandler(AppDbContext db)
    : IRequestHandler<GetRadiologyDetailsQuery, RadiologyDetailsResponse?>
{
    public async Task<RadiologyDetailsResponse?> Handle(GetRadiologyDetailsQuery request, CancellationToken ct)
    {
        var b = await db.Businesses
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.Type == BusinessType.Radiology)
            .Select(x => new
            {
                x.Id, x.Name, x.ProfileImageUrl, x.CoverImageUrl,
                x.Description, x.Address, x.Latitude, x.Longitude,
                CityName        = x.City.Name,
                CityId          = x.City.Id,
                GovernorateName = x.City.Governorate.Name,
                GovernorateId   = x.City.Governorate.Id,
                AvgRating    = x.Testimonials.Select(t => (double)t.Rating).DefaultIfEmpty().Average(),
                TotalRatings = x.Testimonials.Count,
                WorkingDays  = x.WorkingDays.Select(w => new { w.Day, w.StartTime, w.EndTime }).ToList(),
                PhoneNumbers = x.PhoneNumbers.Select(p => new { p.Number, p.Type }).ToList(),
                Branches = x.Branches.Select(br => new
                {
                    br.Id, br.ParentBusinessId, br.Name, br.Address,
                    br.Latitude, br.Longitude,
                    GovernorateName = br.City.Governorate.Name,
                    CityName        = br.City.Name,
                    PhoneNumbers    = br.PhoneNumbers.Select(p => new { p.Number, p.Type }).ToList(),
                    WorkingDays     = br.WorkingDays.Select(w => new { w.Day, w.StartTime, w.EndTime }).ToList(),
                }).ToList(),
                Testimonials = x.Testimonials.OrderByDescending(t => t.CreatedAt).Take(5)
                    .Select(t => new { t.Id, t.User.FullName, t.User.ImageUrl, t.Rating, t.Text, t.CreatedAt }).ToList(),
                Services = x.BusinessServices.Select(bs => bs.Service.Name).ToList(),
            })
            .FirstOrDefaultAsync(ct);

        if (b is null) return null;

        var today = DateTime.UtcNow.DayOfWeek;
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);

        return new RadiologyDetailsResponse(
            b.Id, b.Name, b.ProfileImageUrl, b.CoverImageUrl, b.Description, b.Address,
            b.GovernorateName, b.CityName, b.GovernorateId, b.CityId, b.Latitude, b.Longitude,
            Math.Round(b.AvgRating, 1), b.TotalRatings,
            b.WorkingDays
                .Select(w => new WorkingDayDto((int)w.Day, w.StartTime.ToString("HH:mm"), w.EndTime.ToString("HH:mm")))
                .OrderBy(w => w.Day).ToList(),
            b.PhoneNumbers.Select(p => new PhoneNumberDto(p.Number, p.Type)).ToList(),
            b.Branches.Select(br =>
            {
                var todayRecord = br.WorkingDays.FirstOrDefault(w => w.Day == today);
                var isOpen = todayRecord is not null && todayRecord.StartTime <= now && todayRecord.EndTime >= now;
                return new BranchDto(
                    br.Id, br.ParentBusinessId, br.Name, br.Address,
                    br.GovernorateName, br.CityName,
                    null, br.Latitude, br.Longitude, isOpen,
                    br.PhoneNumbers.Select(p => new PhoneNumberDto(p.Number, p.Type)).ToList(),
                    br.WorkingDays.Select(w => new WorkingDayDto((int)w.Day, w.StartTime.ToString("HH:mm"), w.EndTime.ToString("HH:mm"))).OrderBy(w => w.Day).ToList()
                );
            }).ToList(),
            b.Testimonials.Select(t => new TestimonialDto(t.Id, t.FullName, t.ImageUrl, t.Rating, t.Text, t.CreatedAt)).ToList(),
            b.Services.Select(s => new ServiceDto(s)).ToList()
        );
    }
}
