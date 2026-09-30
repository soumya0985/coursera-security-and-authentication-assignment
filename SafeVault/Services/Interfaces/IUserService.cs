using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeVault.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<IdentityUser>> GetUserListAsync();
    }
}