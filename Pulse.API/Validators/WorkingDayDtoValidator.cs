using FluentValidation;
using Pulse.API.Features.Shared;

namespace Pulse.API.Validators;

public class WorkingDayDtoValidator : AbstractValidator<WorkingDayDto>
{
    public WorkingDayDtoValidator()
    {
        RuleFor(x => x.Day).InclusiveBetween(0, 6);
        RuleFor(x => x.StartTime).NotEmpty().Matches(@"^([01]\d|2[0-3]):[0-5]\d$");
        RuleFor(x => x.EndTime).NotEmpty().Matches(@"^([01]\d|2[0-3]):[0-5]\d$");
        RuleFor(x => x).Must(x =>
            TimeSpan.TryParseExact(x.StartTime, "hh\\:mm", null, out var start) &&
            TimeSpan.TryParseExact(x.EndTime, "hh\\:mm", null, out var end) &&
            start < end)
            .WithMessage("StartTime must be before EndTime");
    }
}
