using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Radiology.CreateRadiology;
using Pulse.API.Persistence;

namespace Pulse.API.Validators;

public class CreateRadiologyCommandValidator : AbstractValidator<CreateRadiologyCommand>
{
    public CreateRadiologyCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CityId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<City>().AnyAsync(c => c.Id == id, ct))
            .WithMessage("City does not exist");
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
        RuleForEach(x => x.Services).SetValidator(new BusinessServiceItemValidator());
    }
}
