using FluentValidation;
using Pulse.API.Features.Users.ChangePassword;

namespace Pulse.API.Features.Users.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.UserId).NotEmpty();
    }
}
