using Microsoft.AspNetCore.Identity;
using NotifyBotApi.Interfaces;

namespace NotifyBotApi.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public string[] GetApplicationRoles()
        {
            var roles = roleManager.Roles.Select(r => r.Name).ToArray();
            return roles;
        }

        
    }
}
