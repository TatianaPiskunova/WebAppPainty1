using Microsoft.AspNetCore.Identity;
using WebAppPainty1;

namespace BlogTestApp
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(ApiContext context, UserManager<User> userManager, 
           RoleManager<IdentityRole> roleManager)

        {
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            

            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
             
            }
         

        }
    }
}
