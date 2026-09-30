namespace SafeVault.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public interface IUserAuthorizationService
{
    Task<bool> UserCanManageUsersAsync(IdentityUser user);
}
