using Pulse.API.Domain.Entities;

namespace Pulse.API.Features.Shared;

/// <summary>
/// Computes today's open status and the next working day from raw WorkingDay rows.
/// Keeps schedule logic in one place, reusable across all business types (doctors, pharmacies, labs).
/// The frontend receives raw DayOfWeek/TimeOnly values and handles Arabic formatting.
/// </summary>
public static class DoctorScheduleHelper
{
    /// <param name="workingDays">All WorkingDay rows for a single business (up to 7 rows).</param>
    /// <param name="today">UTC day of week.</param>
    /// <param name="now">UTC time.</param>
    public static DoctorScheduleDto Compute(IEnumerable<WorkingDay> workingDays, DayOfWeek today, TimeOnly now)
    {
        var days = workingDays.ToList();

        // --- Is the business open right now? ---
        var todayRecord = days.FirstOrDefault(w => w.Day == today);
        var isOpen = todayRecord != null && todayRecord.StartTime <= now && todayRecord.EndTime >= now;

        // --- Find the next working day (scan up to 7 days ahead) ---
        DayOfWeek? nextDay = null;
        TimeOnly? nextStart = null;
        TimeOnly? nextEnd = null;

        for (var offset = 1; offset <= 7; offset++)
        {
            // ((int)today + offset) % 7 wraps around the week
            var checkDay = (DayOfWeek)(((int)today + offset) % 7);
            var wd = days.FirstOrDefault(w => w.Day == checkDay);
            if (wd == null) continue;

            nextDay = checkDay;
            nextStart = wd.StartTime;
            nextEnd = wd.EndTime;
            break;
        }

        return new DoctorScheduleDto(
            isOpen,
            todayRecord?.StartTime,
            todayRecord?.EndTime,
            (int?)nextDay,
            nextStart,
            nextEnd
        );
    }
}

/// <summary>
/// Raw schedule data — no Arabic formatting, no display logic.
/// The frontend converts these to "من ٩ صباحاً - ٩ مساءً" or "غداً: ٩ صباحاً - ٥ مساءً".
/// </summary>
public record DoctorScheduleDto(
    bool IsOpen,
    TimeOnly? TodayStart,
    TimeOnly? TodayEnd,
    int? NextDayOfWeek,   // 0=Sunday … 6=Saturday
    TimeOnly? NextStart,
    TimeOnly? NextEnd
);
