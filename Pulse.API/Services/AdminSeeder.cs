using Pulse.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Pulse.API.Services;

public static class AdminSeeder
{
    private const string AdminRole = "Admin";
    private const string ManagerRole = "Manager";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // 1. Ensure roles exist
        foreach (var roleName in new[] { AdminRole, ManagerRole })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole<Guid> { Name = roleName, NormalizedName = roleName.ToUpper() });
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create {roleName} role: {errors}");
                }
            }
        }

        // 3. Ensure the admin user exists
        const string adminEmail = "admin@pulse.com";
        const string adminPassword = "Admin@123456";
        const string adminFullName = "System Admin";

        var existing = await userManager.FindByEmailAsync(adminEmail);

        if (existing == null)
        {
            var admin = new ApplicationUser
            {
                Id = new Guid("00000000-0000-0000-0000-0000000000AD"),
                Email = adminEmail,
                UserName = adminEmail,
                FullName = adminFullName,
                EmailConfirmed = true,
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed admin user: {errors}");
            }

            existing = admin;
        }

        // 4. Assign Admin role if not already assigned
        if (!await userManager.IsInRoleAsync(existing, AdminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(existing, AdminRole);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to assign Admin role: {errors}");
            }
        }
    }
}
