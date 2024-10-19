using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using HKShared.Data;
using Microsoft.EntityFrameworkCore;

namespace HKShared.Helpers
{
    public static class AppSettings
    {
        public static StringMap Strings;

        static AppSettings()
        {
            Strings = new StringMap();
        }

        public static IApplicationBuilder InitAppSettings(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<AppDBContext>();
                InitData(serviceScope);
                LoadData(dbContext);
            }

            return app;
        }

        public static void InitData(IServiceScope serviceScope)
        {
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<AppRole>>();
            SeedRoles(roleManager);

            var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
            SeedUsers(userManager);
        }
        public static void LoadData(AppDBContext dbContext)
        {
            Strings.Clear();
            foreach (var item in dbContext.AppSettings)
                Strings[item.Name] = item.Value;
        }

        public static void SeedRoles(RoleManager<AppRole> roleManager)
        {

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new AppRole("Admin");
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Manager").Result)
            {
                var role = new AppRole("Manager");
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Operator").Result)
            {
                var role = new AppRole("Operator");
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }

        public static void SeedUsers(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("Admin").Result == null)
            {
                var user = new AppUser()
                {
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    Email = "tadlhvuong@gmail.com",
                    CreateTime = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    EmailConfirmed = true,
                    Status = EntityStatus.Enabled,
                };

                var result = userManager.CreateAsync(user, "AdminP@ssW0rd123").Result;
                if (result.Succeeded)
                {
                    var roles = new string[] { "Administrator", "Manager", "Operator" };
                    userManager.AddToRolesAsync(user, roles).Wait();
                }
            }
        }
    }
}
