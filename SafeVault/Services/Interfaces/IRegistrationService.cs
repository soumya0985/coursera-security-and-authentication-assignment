using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SafeVault.Services.Interfaces
{
	public interface IUserRegistrationService
	{
		Task<IdentityResult> RegisterUserAsync(
            string username,
            string email,
            string password,
            string role);

		Task<bool> CheckRegisteredUserAsync(string username, string email);
	}
}
