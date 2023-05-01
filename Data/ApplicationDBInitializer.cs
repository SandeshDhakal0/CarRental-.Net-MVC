using Microsoft.AspNetCore.Identity;
using HamroCarRental.Models;
using HamroCarRental.Models.Identity;

namespace HamroCarRental.Data;

public class ApplicationDBInitializer
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Assistant = "Assistant";
    }


    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            // Roles
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Manager))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));

            if (!await roleManager.RoleExistsAsync(UserRoles.Assistant))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Assistant));


            // Users
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminUserEmail = "admin@ropey.com";
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    FullName = "Ropey Admin",
                    UserName = "AdminUser",
                    Email = adminUserEmail,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Manager);
            }


            var appUserEmail = "user@ropey.com";
            var appUser = await userManager.FindByEmailAsync(appUserEmail);
            if (appUser == null)
            {
                var newAppUser = new ApplicationUser
                {
                    FullName = "Ropey Assistant",
                    UserName = "app-user",
                    Email = appUserEmail,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAppUser, "Coding@1234?");
                await userManager.AddToRoleAsync(newAppUser, UserRoles.Assistant);
            }
        }
    }


}