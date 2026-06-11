using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure.Exceptions;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Shared mapping helpers for Doctor and Branch write operations.
/// Eliminates duplicated WorkingDay / PhoneNumber mapping and validation.
/// </summary>
public static class DoctorMappingHelpers
{
    /// <summary>
    /// Validates and maps WorkingDayDto list to WorkingDay entities.
    /// Throws BadRequestException on invalid time format or start >= end.
    /// Returns empty list when input is null.
    /// </summary>
    public static List<WorkingDay> MapWorkingDays(List<WorkingDayDto>? dtos)
    {
        if (dtos is null or { Count: 0 }) return [];

        var result = new List<WorkingDay>(dtos.Count);
        foreach (var wd in dtos)
        {
            if (wd.Day is < 0 or > 6)
                throw new BadRequestException($"Invalid day value '{wd.Day}'. Must be 0-6 (Sunday-Saturday)");

            if (!TimeOnly.TryParse(wd.StartTime, out var start))
                throw new BadRequestException($"Invalid start time '{wd.StartTime}' for day {wd.Day}. Expected HH:mm");

            if (!TimeOnly.TryParse(wd.EndTime, out var end))
                throw new BadRequestException($"Invalid end time '{wd.EndTime}' for day {wd.Day}. Expected HH:mm");

            if (start >= end)
                throw new BadRequestException($"Start time must be before end time for day {wd.Day}");

            result.Add(new WorkingDay
            {
                Day       = (System.DayOfWeek)wd.Day,
                StartTime = start,
                EndTime   = end,
            });
        }
        return result;
    }

    /// <summary>
    /// Validates and maps PhoneNumberDto list to PhoneNumber entities.
    /// Returns empty list when input is null.
    /// </summary>
    public static List<PhoneNumber> MapPhoneNumbers(List<PhoneNumberDto>? dtos)
    {
        if (dtos is null or { Count: 0 }) return [];

        var result = new List<PhoneNumber>(dtos.Count);
        foreach (var pn in dtos)
        {
            if (string.IsNullOrWhiteSpace(pn.Number))
                throw new BadRequestException("Phone number cannot be empty");

            result.Add(new PhoneNumber
            {
                Number = pn.Number.Trim(),
                Type   = pn.Type?.Trim(),
            });
        }
        return result;
    }
}
