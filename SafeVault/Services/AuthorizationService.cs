using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace SafeVault.Services;

using SafeVault.Services.Interfaces;
public class AuthorizationService : IUserAuthorizationService
{
    private readonly UserManager<IdentityUser> _userManager;

    public AuthorizationService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Created to simulate user authorization based on roles.
    /// Because middleware typically handles authorization, 
    /// this method is primarily for testing and simulation purposes.
    /// Determines whether the specified user has the "admin" role and can manage other users.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<bool> UserCanManageUsersAsync(
        IdentityUser user)
    {
        return await _userManager.IsInRoleAsync(
            user,
            "admin");
    }
}