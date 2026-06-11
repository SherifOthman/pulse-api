using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Cities.CreateCity;
using Pulse.API.Persistence;

namespace Pulse.API.Features.Cities.CreateCity;

public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.GovernorateId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<Governorate>().AnyAsync(g => g.Id == id, ct))
            .WithMessage("Governorate does not exist");
    }
}
