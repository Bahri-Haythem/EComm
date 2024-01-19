using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Identity;

public class AppIdentityDbContextSeed
{
    public static async Task SeedUserAsync(UserManager<AppUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new AppUser
            {
                DisplayName = "Test",
                Email = "test@example.com",
                UserName = "test@example.com",
                Address = new Address
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Street = "10 The Street",
                    City = "New York",
                    State = "NY",
                    ZipCode = "90210"
                }
            };

            await userManager.CreateAsync(user, "Pa$w0rd*");
        }
    }
}
