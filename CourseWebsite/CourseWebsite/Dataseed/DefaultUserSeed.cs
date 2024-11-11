using CourseWebsite.Constants;
using CourseWebsite.Data;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace CourseWebsite.Dataseed
{
    public static class DefaultUserSeed
    {
        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultuser = new ApplicationUser
            {
                UserName = "Marwan",
                Email = "FFMS@FFMS.com",
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(defaultuser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultuser, "P@ssw0rd");
                await userManager.AddToRoleAsync(defaultuser, Roles.SuperAdminRole);

            }
        }
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {

            await SeedRoleAsync(roleManager, Roles.SuperAdminRole);
            //await SeedRoleAsync(roleManager, Roles.adminRole);
            await SeedRoleAsync(roleManager, Roles.NormalRole);


            /*  var role = new IdentityRole
              {
                  Name=Roles.SuperAdminRole,     
              };
              var roleExist = await roleManager.FindByNameAsync(role.Name);
              if (roleExist == null)
              {
                  await roleManager.CreateAsync(role);
              }*/
        }

        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            var role = new IdentityRole
            {
                Name = roleName,
            };
            var roleExist = await roleManager.FindByNameAsync(role.Name);
            if (roleExist == null)
            {
                await roleManager.CreateAsync(role);
            }
        }




        public static IServiceProvider Migrate(this IServiceProvider serviceProvider)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                // Use the dbContext instance as needed
            }

            return serviceProvider;

        }




    }
}
