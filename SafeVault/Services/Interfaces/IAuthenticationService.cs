namespace SafeVault.Services.Interfaces;
using System.Threading.Tasks;
public interface IUserAuthenticationService
{
	Task<bool> LoginAsync(string username, string password);
	Task LogoutAsync();
}
