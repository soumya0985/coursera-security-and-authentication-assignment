using Microsoft.AspNetCore.Identity;
using SafeVault.Models;
using System.Threading.Tasks;

namespace SafeVault.Helpers
{
    public static class SeedDataHelper
    {
        // Methods for seeding initial data for roles and users can be added here
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User));
            }
        }

        public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByNameAsync("alice") is null)
            {
                var user = new IdentityUser
                {
                    UserName = "alice",
                    Email = "alice@example.com"
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, Roles.Admin);
            }

            if (await userManager.FindByNameAsync("bob") is null)
            {
                var user = new IdentityUser
                {
                    UserName = "bob",
                    Email = "bob@example.com"
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, Roles.User);
            }

            if (await userManager.FindByNameAsync("charlie") is null)
            {
                var user = new IdentityUser
                {
                    UserName = "charlie",
                    Email = "charlie@example.com"
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, Roles.User);
            }
        }
    }
}