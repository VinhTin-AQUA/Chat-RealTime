using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotifyBotApi.Data;
using NotifyBotApi.Models;
using System.Security.Claims;

namespace NotifyBotApi.Services
{
    public class ContextSeedService
    {
        private readonly NotifyBotContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ContextSeedService(
            NotifyBotContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            // kiểm tra có migration nào ở trạng thái pending không
            if (context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                // tiến hành cập nhật migration
                await context.Database.MigrateAsync();
            }

            // seed role roles
            if (await roleManager.Roles.AnyAsync() == false)
            {
                await roleManager.CreateAsync(new IdentityRole(SeedData.AdminRole));
                await roleManager.CreateAsync(new IdentityRole(SeedData.ManagerRole));
                await roleManager.CreateAsync(new IdentityRole(SeedData.UserRole));
            }

            // seed user
            if (await userManager.Users.AnyAsync() == false)
            {
                // admin
                var admin = new AppUser()
                {
                    UserName = SeedData.AdminEmail,
                    Email = SeedData.AdminEmail,
                    EmailConfirmed = true,
                    DateCreated = DateTime.UtcNow,
                    FirstName = SeedData.AdminFirstName,
                    LastName = SeedData.AdminLastName,
                };
  
                await userManager.CreateAsync(admin,SeedData.AdminPassword);
               
                await userManager.AddToRolesAsync(admin, new[] { SeedData.AdminRole });
                await userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(ClaimTypes.Email, admin.Email),
                });
            }
        }
        
    }
}
