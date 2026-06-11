using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pulse.API.Domain.Entities;
using Pulse.API.Features.Doctors.UpdateDoctor;
using Pulse.API.Persistence;

namespace Pulse.API.Validators;

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Name).MaximumLength(250).When(x => x.Name is not null);
        RuleFor(x => x.CityId).MustAsync(async (id, ct) => await db.Set<City>().AnyAsync(c => c.Id == id, ct))
            .WithMessage("City does not exist").When(x => x.CityId.HasValue);
        RuleFor(x => x.SpecializationId).MustAsync(async (id, ct) => await db.Set<Specialization>().AnyAsync(s => s.Id == id, ct))
            .WithMessage("Specialization does not exist").When(x => x.SpecializationId.HasValue);
        RuleFor(x => x.Gender).IsInEnum().When(x => x.Gender is not null);
        RuleFor(x => x.VisitPrice).GreaterThanOrEqualTo(0).When(x => x.VisitPrice.HasValue);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
        RuleForEach(x => x.WorkingDays).SetValidator(new WorkingDayDtoValidator());
        RuleForEach(x => x.PhoneNumbers).SetValidator(new PhoneNumberDtoValidator());
    }
}
