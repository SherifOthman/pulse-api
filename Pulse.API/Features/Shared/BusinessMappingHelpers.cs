using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Infrastructure.Exceptions;
using Pulse.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Shared helpers for creating and updating the common Business fields
/// (Name, CityId, Address, Description, ProfileImageUrl, CoverImageUrl, Lat/Lng).
/// Used by Pharmacy, Lab, and Radiology handlers to eliminate copy-paste.
/// Doctor has its own handler due to the additional Doctor navigation property.
/// </summary>
public static class BusinessMappingHelpers
{
    /// <summary>
    /// Validates and creates a new Business entity with base fields populated.
    /// </summary>
    public static async Task<Business> CreateBusinessAsync(
        AppDbContext db,
        string name,
        Guid? cityId,
        string? address,
        string? description,
        string? profileImageUrl,
        string? coverImageUrl,
        double? latitude,
        double? longitude,
        BusinessType type,
        Guid createdByUserId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Name is required");

        if (cityId.HasValue && !await db.Set<City>().AnyAsync(c => c.Id == cityId.Value, ct))
            throw new NotFoundException("City not found");

        return new Business
        {
            Name            = name.Trim(),
            Type            = type,
            CityId          = cityId ?? Guid.Empty,
            Address         = address?.Trim(),
            Description     = description?.Trim(),
            ProfileImageUrl = profileImageUrl?.Trim(),
            CoverImageUrl   = coverImageUrl?.Trim(),
            Latitude        = latitude,
            Longitude       = longitude,
            CreatedByUserId = createdByUserId,
        };
    }

    /// <summary>
    /// Applies partial updates to an existing Business entity.
    /// Only non-null values are applied (patch semantics).
    /// </summary>
    public static async Task ApplyUpdatesAsync(
        AppDbContext db,
        Business business,
        string? name,
        Guid? cityId,
        string? address,
        string? description,
        string? profileImageUrl,
        string? coverImageUrl,
        double? latitude,
        double? longitude,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(name))
            business.Name = name.Trim();

        if (cityId.HasValue)
        {
            if (!await db.Set<City>().AnyAsync(c => c.Id == cityId.Value, ct))
                throw new NotFoundException("City not found");
            business.CityId = cityId.Value;
        }

        if (address is not null)
            business.Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();

        if (description is not null)
            business.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (profileImageUrl is not null)
            business.ProfileImageUrl = string.IsNullOrWhiteSpace(profileImageUrl) ? null : profileImageUrl.Trim();

        if (coverImageUrl is not null)
            business.CoverImageUrl = string.IsNullOrWhiteSpace(coverImageUrl) ? null : coverImageUrl.Trim();

        if (latitude.HasValue)  business.Latitude  = latitude;
        if (longitude.HasValue) business.Longitude = longitude;
    }

    /// <summary>
    /// Projects a Business row into the shared detail response sub-parts.
    /// Call from each GetDetails handler to avoid repeating the LINQ projection.
    /// </summary>
    public static BusinessDetailProjection ProjectDetails(
        IEnumerable<WorkingDayDto> workingDays,
        IEnumerable<PhoneNumberDto> phoneNumbers,
        IEnumerable<BranchDto> branches,
        IEnumerable<TestimonialDto> testimonials,
        IEnumerable<ServiceDto> services) =>
        new(workingDays.ToList(), phoneNumbers.ToList(),
            branches.ToList(), testimonials.ToList(), services.ToList());
}

public record BusinessDetailProjection(
    List<WorkingDayDto> WorkingDays,
    List<PhoneNumberDto> PhoneNumbers,
    List<BranchDto> Branches,
    List<TestimonialDto> Testimonials,
    List<ServiceDto> Services
);
