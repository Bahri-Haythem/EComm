using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AppUser> FindUserByEmailWithAddressAsync(this UserManager<AppUser> userManager, string email)
    {
        return await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
    }
}
