using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared;
using Pulse.API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Laboratories.GetMobileLaboratoryDetails;

public class GetMobileLaboratoryDetailsHandler(AppDbContext db)
    : IRequestHandler<GetMobileLaboratoryDetailsQuery, LaboratoryMobileDetailsResponse?>
{
    public async Task<LaboratoryMobileDetailsResponse?> Handle(
        GetMobileLaboratoryDetailsQuery request, CancellationToken ct)
    {
        var userId = request.CurrentUserId;

        var b = await db.Businesses
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.Type == BusinessType.Laboratory)
            .Select(x => new
            {
                x.Id, x.Name, x.ProfileImageUrl, x.CoverImageUrl,
                x.Description, x.Address,
                CityName        = x.City.Name,
                GovernorateName = x.City.Governorate.Name,
                AvgRating    = x.Testimonials.Select(t => (double)t.Rating).DefaultIfEmpty().Average(),
                TotalRatings = x.Testimonials.Count,
                IsFavorite   = userId != null && db.UserFavorite.Any(f => f.UserId == userId.Value && f.BuissnessId == x.Id),
                HasUserReviewed = userId != null && x.Testimonials.Any(t => t.UserId == userId.Value),
                WorkingDays  = x.WorkingDays.Select(w => new { w.Day, w.StartTime, w.EndTime }).ToList(),
                PhoneNumbers = x.PhoneNumbers.Select(p => new { p.Number, p.Type }).ToList(),
                Branches     = x.Branches.Select(br => new
                {
                    br.Id, br.Name, br.Address, br.ProfileImageUrl,
                    PhoneNumbers = br.PhoneNumbers.Select(p => new { p.Number, p.Type }).ToList(),
                    WorkingDays  = br.WorkingDays.Select(w => new { w.Day, w.StartTime, w.EndTime }).ToList()
                }).ToList(),
                Testimonials = x.Testimonials.OrderByDescending(t => t.CreatedAt).Take(3)
                    .Select(t => new { t.Id, t.User.FullName, t.User.ImageUrl, t.Rating, t.Text, t.CreatedAt }).ToList(),
                Services = x.BusinessServices.Select(bs => bs.Service.Name).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (b is null) return null;

        return new LaboratoryMobileDetailsResponse(
            b.Id, b.Name, b.ProfileImageUrl, b.CoverImageUrl,
            b.Description, b.Address,
            b.GovernorateName, b.CityName,
            Math.Round(b.AvgRating, 1), b.TotalRatings, b.IsFavorite, b.HasUserReviewed,
            b.WorkingDays.Select(w => new WorkingDayDto((int)w.Day, w.StartTime.ToString("HH:mm"), w.EndTime.ToString("HH:mm"))).OrderBy(w => w.Day).ToList(),
            b.PhoneNumbers.Select(p => new PhoneNumberDto(p.Number, p.Type)).ToList(),
            b.Branches.Select(br => new BranchDto(
                br.Id, br.Name, br.Address, br.ProfileImageUrl,
                br.PhoneNumbers.Select(p => new PhoneNumberDto(p.Number, p.Type)).ToList(),
                br.WorkingDays.Select(w => new WorkingDayDto((int)w.Day, w.StartTime.ToString("HH:mm"), w.EndTime.ToString("HH:mm"))).OrderBy(w => w.Day).ToList()
            )).ToList(),
            b.Testimonials.Select(t => new TestimonialDto(t.Id, t.FullName, t.ImageUrl, t.Rating, t.Text, t.CreatedAt)).ToList(),
            b.Services.Select(s => new ServiceDto(s)).ToList()
        );
    }
}
