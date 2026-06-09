using Pulse.API.Domain.Entities;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Common query extensions for all business types (Doctor, Pharmacy, Lab, Radiology).
/// </summary>
public static class BusinessQueryExtensions
{
    /// <summary>
    /// Applies common filters: governorate, city, name search.
    /// </summary>
    public static IQueryable<Business> ApplyFilters(
        this IQueryable<Business> query,
        BusinessQuery bq)
    {
        if (bq.GovernorateId.HasValue)
            query = query.Where(b => b.City.GovernorateId == bq.GovernorateId.Value);

        if (bq.CityId.HasValue)
            query = query.Where(b => b.CityId == bq.CityId.Value);

        if (!string.IsNullOrWhiteSpace(bq.Name))
            query = query.Where(b => b.Name.Contains(bq.Name));

        return query;
    }
}
