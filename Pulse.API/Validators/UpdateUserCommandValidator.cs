using FluentValidation;
using Pulse.API.Features.Users.UpdateUser;

namespace Pulse.API.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FullName).MaximumLength(100).When(x => x.FullName is not null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(254).When(x => x.Email is not null);
        RuleFor(x => x.Password).MinimumLength(6).When(x => x.Password is not null);
        RuleFor(x => x.Role).MaximumLength(50).When(x => x.Role is not null);
    }
}
