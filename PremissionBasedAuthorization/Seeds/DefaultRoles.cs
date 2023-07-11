using Microsoft.AspNetCore.Identity;
using PremissionBasedAuthorization.Constants;

namespace PremissionBasedAuthorization.Seeds
{
    public static class DefaultRoles
    {
        public static async Task seedAsync(RoleManager<IdentityRole> roleManager)
        {
           if(!roleManager.Roles.Any())
           {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
           }

        }
    }
}
