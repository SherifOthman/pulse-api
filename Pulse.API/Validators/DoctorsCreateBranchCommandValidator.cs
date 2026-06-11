using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Domain.Enums;
using Pulse.API.Features.Doctors.Branches.CreateBranch;
using Pulse.API.Persistence;

namespace Pulse.API.Validators;

public class DoctorsCreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public DoctorsCreateBranchCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.DoctorId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<Business>()
                .AnyAsync(b => b.Id == id && b.Type == BusinessType.Doctor, ct))
            .WithMessage("Doctor does not exist");
        RuleFor(x => x.CityId).NotEmpty()
            .MustAsync(async (id, ct) => await db.Set<City>().AnyAsync(c => c.Id == id, ct))
            .WithMessage("City does not exist");
        RuleFor(x => x.VisitPrice).GreaterThanOrEqualTo(0).When(x => x.VisitPrice.HasValue);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
        RuleForEach(x => x.WorkingDays).SetValidator(new WorkingDayDtoValidator());
        RuleForEach(x => x.PhoneNumbers).SetValidator(new PhoneNumberDtoValidator());
    }
}
