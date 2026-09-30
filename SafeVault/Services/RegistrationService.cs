using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SafeVault.Configuration;
using SafeVault.Helpers;
using SafeVault.Services.Interfaces;

namespace SafeVault.Services
{
    public class RegistrationService : IUserRegistrationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ValidationOptions _validationOptions;

        public RegistrationService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<ValidationOptions> validationOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _validationOptions = validationOptions.Value;
        }

        

        public async Task<bool> CheckRegisteredUserAsync(
            string username,
            string email)
        {
            if (!ValidationHelper.IsValidInput(username, _validationOptions.AllowedUsernameCharacters) ||
                !ValidationHelper.IsValidInput(email, _validationOptions.AllowedEmailCharacters))
            {
                return false;
            }

            var user = await _userManager.FindByNameAsync(username);

            return user != null &&
                string.Equals(
                    user.Email,
                    email,
                    StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IdentityResult> RegisterUserAsync(
            string username,
            string email,
            string password,
            string role)
        {
            if (!ValidationHelper.IsValidInput(username, _validationOptions.AllowedUsernameCharacters) ||
                !ValidationHelper.IsValidInput(email, _validationOptions.AllowedEmailCharacters) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Invalid registration input."
                });
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Role does not exist."
                });
            }

            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Username is already registered."
                });
            }

            var user = new IdentityUser
            {
                UserName = username,
                Email = email
            };

            // ASP.NET Core Identity handles password hashing and validation requirements,
            // so this service only validates the username and email format before creating the user.
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                return createResult;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role.ToLowerInvariant());
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return roleResult;
            }

            return IdentityResult.Success;
        }
    }
}