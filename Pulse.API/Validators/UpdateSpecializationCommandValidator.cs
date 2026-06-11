using FluentValidation;
using Pulse.API.Features.Specializations.UpdateSpecialization;

namespace Pulse.API.Validators;

public class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100).When(x => x.Name is not null);
    }
}
