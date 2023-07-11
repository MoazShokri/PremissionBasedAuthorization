using Microsoft.AspNetCore.Identity;
using PremissionBasedAuthorization.Constants;
using System.Security.Claims;

namespace PremissionBasedAuthorization.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedBasicUserAsync(UserManager<IdentityUser> userManager)
        {
            var DefaultUser = new IdentityUser
            {
                UserName = "basicuser@domain.com",
                Email = "basicuser@domain.com",
                EmailConfirmed= true 
            };
            var user = await userManager.FindByEmailAsync(DefaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(DefaultUser , "P@ssword123");
                await userManager.AddToRoleAsync(DefaultUser,Roles.Basic.ToString());
            }
        }
        public static async Task SeedSuperAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var DefaultUser = new IdentityUser
            {
                UserName = "superadmin@domain.com",
                Email = "superadmin@domain.com",
                EmailConfirmed = true
            };
            var user = await userManager.FindByEmailAsync(DefaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(DefaultUser , "P@ssword123");
                await userManager.AddToRolesAsync(DefaultUser, new List<string> {Roles.Basic.ToString() , Roles.SuperAdmin.ToString()});
            }
            await roleManager.SeedClaimsForSuperUser();
        }

        private static async Task SeedClaimsForSuperUser(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(adminRole, "Products");
        }

        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsList(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
        
    }
}
