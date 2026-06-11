using FluentValidation;
using Pulse.API.Features.Specializations.UpdateSpecialization;

namespace Pulse.API.Validators;

public class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(250).When(x => x.Name is not null);
    }
}
