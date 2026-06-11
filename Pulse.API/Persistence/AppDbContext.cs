using Pulse.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<UserFavorite> UserFavorite => Set<UserFavorite>();
        public DbSet<Testimonial> Testimonials => Set<Testimonial>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Governorate> Governorates => Set<Governorate>();
        public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
        public DbSet<PharmacyProfile> PharmacyProfiles => Set<PharmacyProfile>();
        public DbSet<LaboratoryProfile> LaboratoryProfiles => Set<LaboratoryProfile>();
        public DbSet<RadiologyProfile> RadiologyProfiles => Set<RadiologyProfile>();
        public DbSet<Specialization> Specializations => Set<Specialization>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<WorkingDay> WorkingDays => Set<WorkingDay>();
        public DbSet<PhoneNumber> PhoneNumbers => Set<PhoneNumber>();
        public DbSet<BusinessService> BusinessServices => Set<BusinessService>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(builder);
        }
    }
}
