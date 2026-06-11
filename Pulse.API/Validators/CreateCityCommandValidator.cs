using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Cities.CreateCity;
using Pulse.API.Persistence;

namespace Pulse.API.Validators;

public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.GovernorateId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<Governorate>().AnyAsync(g => g.Id == id, ct))
            .WithMessage("Governorate does not exist");
    }
}
