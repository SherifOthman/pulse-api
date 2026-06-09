namespace Pulse.API.Features.Favorites;

/// <summary>
/// Lightweight response for the favorites list.
/// Only what the card needs — name, image, rating.
/// Full details (schedule, price, etc.) fetched separately on the detail page.
/// </summary>
public record FavoriteListItemResponse(
    Guid Id,
    string Name,
    string? ProfileImageUrl,
    double AverageRating,
    int TotalRatings,
    int BusinessType   // 1=Doctor, 2=Pharmacy, 3=Laboratory, 4=Radiology
);
