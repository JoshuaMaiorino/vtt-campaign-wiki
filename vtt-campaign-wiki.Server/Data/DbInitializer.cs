using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using vtt_campaign_wiki.Server.Features.Player;

namespace vtt_campaign_wiki.Server.Data
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAsync( IServiceProvider serviceProvider )
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<PlayerEntity>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Ensure the admin role exists
            const string adminRoleName = "Admin";
            if (!await roleManager.RoleExistsAsync( adminRoleName ))
            {
                await roleManager.CreateAsync( new IdentityRole<int> { Name = adminRoleName } );
            }

            // Ensure an admin user exists
            const string adminUserName = "admin";
            const string adminPassword = "P@ssword123!";

            var adminUser = await userManager.FindByNameAsync( adminUserName );
            if (adminUser == null)
            {
                adminUser = new PlayerEntity
                {
                    UserName = adminUserName,
                    Email = "Joshua.Maiorino@Gmail.com",
                    FirstName = "Joshua",
                    LastName = "Maiorino"
                };

                var result = await userManager.CreateAsync( adminUser, adminPassword );
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync( adminUser, adminRoleName );
                }
                else
                {
                    // Handle failure case
                    throw new Exception( $"Failed to create admin user: {string.Join( ", ", result.Errors )}" );
                }
            }
        }
    }
}
