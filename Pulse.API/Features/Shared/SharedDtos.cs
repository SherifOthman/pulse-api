namespace Pulse.API.Features.Shared;

/// <summary>
/// Shared DTOs used across all business types in both list and detail responses.
/// </summary>

public record WorkingDayDto(
    int Day,          // 0=Sunday … 6=Saturday
    string StartTime, // "HH:mm"
    string EndTime    // "HH:mm"
);

public record PhoneNumberDto(
    string Number,
    string? Type
);

public record ServiceDto(
    string Name
);

public record BranchDto(
    Guid Id,
    string Name,
    string? Address,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    string? Description,
    string? Governorate,
    string? City,
    decimal? VisitPrice,
    double? Latitude,
    double? Longitude,
    List<PhoneNumberDto> PhoneNumbers,
    List<WorkingDayDto> WorkingDays
);

public record CreateBranchDto(
    string Name,
    Guid? CityId,
    string? Address,
    string? Description,
    string? ProfileImageUrl,
    string? CoverImageUrl,
    decimal? VisitPrice,
    double? Latitude,
    double? Longitude,
    List<WorkingDayDto>? WorkingDays,
    List<PhoneNumberDto>? PhoneNumbers
);

public record TestimonialDto(
    Guid Id,
    string? UserName,
    string? UserImageUrl,
    byte Rating,
    string Text,
    DateTimeOffset CreatedAt
);
