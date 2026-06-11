using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Shared.Branches.CreateBranch;
using Pulse.API.Persistence;

using Pulse.API.Validators;
namespace Pulse.API.Features.Shared.Branches.CreateBranch;

public class SharedCreateBranchCommandValidator : AbstractValidator<SharedCreateBranchCommand>
{
    public SharedCreateBranchCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BusinessId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<Business>().AnyAsync(b => b.Id == id, ct))
            .WithMessage("Business does not exist");
        RuleFor(x => x.Type).NotNull().IsInEnum();
        RuleFor(x => x.CityId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<City>().AnyAsync(c => c.Id == id, ct))
            .WithMessage("City does not exist");
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
        RuleForEach(x => x.WorkingDays).SetValidator(new WorkingDayDtoValidator());
        RuleForEach(x => x.PhoneNumbers).SetValidator(new PhoneNumberDtoValidator());
    }
}

