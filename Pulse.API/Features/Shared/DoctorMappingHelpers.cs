using Pulse.API.Domain.Entities;
using Pulse.API.Infrastructure.Exceptions;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Shared mapping helpers for Doctor and Branch write operations.
/// Validates input and maps DTOs to entities. Used by all doctor/branch handlers.
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
        if (dtos is null) return [];

        var result = new List<WorkingDay>(dtos.Count);
        foreach (var wd in dtos)
        {
            if (!TimeOnly.TryParse(wd.StartTime, out var start))
                throw new BadRequestException($"تنسيق وقت البداية غير صحيح '{wd.StartTime}'");

            if (!TimeOnly.TryParse(wd.EndTime, out var end))
                throw new BadRequestException($"تنسيق وقت النهاية غير صحيح '{wd.EndTime}'");

            if (start >= end)
                throw new BadRequestException("وقت البداية يجب أن يكون قبل وقت النهاية");

            if (wd.Day < 0 || wd.Day > 6)
                throw new BadRequestException($"يوم العمل غير صحيح: {wd.Day}");

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
    /// Returns empty list when input is null. Skips blank numbers.
    /// </summary>
    public static List<PhoneNumber> MapPhoneNumbers(List<PhoneNumberDto>? dtos)
    {
        if (dtos is null) return [];

        return dtos
            .Where(pn => !string.IsNullOrWhiteSpace(pn.Number))
            .Select(pn => new PhoneNumber
            {
                Number = pn.Number.Trim(),
                Type   = string.IsNullOrWhiteSpace(pn.Type) ? null : pn.Type.Trim(),
            })
            .ToList();
    }
}
