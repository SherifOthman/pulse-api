using FluentValidation;
using Pulse.API.Features.Shared;

namespace Pulse.API.Validators;

public class PhoneNumberDtoValidator : AbstractValidator<PhoneNumberDto>
{
    public PhoneNumberDtoValidator()
    {
        RuleFor(x => x.Number).NotEmpty();
    }
}
