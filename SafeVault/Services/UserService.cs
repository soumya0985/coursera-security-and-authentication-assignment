using Microsoft.AspNetCore.Identity;
using SafeVault.Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeVault.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<List<IdentityUser>> GetUserListAsync()
        {
            var users = _userManager.Users
                .Select(u => new IdentityUser
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty
                })
                .ToList();

            return Task.FromResult(users);
        }
    }
}