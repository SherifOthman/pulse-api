using FluentValidation;
using Pulse.API.Features.Specializations.CreateSpecialization;

namespace Pulse.API.Features.Specializations.CreateSpecialization;

public class CreateSpecializationCommandValidator : AbstractValidator<CreateSpecializationCommand>
{
    public CreateSpecializationCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
