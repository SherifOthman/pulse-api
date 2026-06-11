using FluentValidation;
using Pulse.API.Features.Shared;

namespace Pulse.API.Validators;

public class BusinessServiceItemValidator : AbstractValidator<BusinessServiceItem>
{
    public BusinessServiceItemValidator()
    {
        RuleFor(x => x).Must(x => x.Id.HasValue || !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Each service must have an Id or a Name");
    }
}
