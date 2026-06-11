using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Cities.UpdateCity;
using Pulse.API.Persistence;

namespace Pulse.API.Validators;

public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).MaximumLength(250).When(x => x.Name is not null);
        RuleFor(x => x.GovernorateId)
            .MustAsync(async (id, ct) => await db.Set<Governorate>().AnyAsync(g => g.Id == id, ct))
            .WithMessage("Governorate does not exist").When(x => x.GovernorateId.HasValue);
    }
}
