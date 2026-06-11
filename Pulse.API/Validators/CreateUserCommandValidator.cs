using FluentValidation;
using Pulse.API.Features.Users.CreateUser;

namespace Pulse.API.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Role).MaximumLength(50).When(x => x.Role is not null);
    }
}
