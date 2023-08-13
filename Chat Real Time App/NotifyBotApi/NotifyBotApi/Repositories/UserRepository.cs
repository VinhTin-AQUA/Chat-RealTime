using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotifyBotApi.Data;
using NotifyBotApi.DTOs.Account;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using NotifyBotApi.Services;

namespace NotifyBotApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly JWTService jwtService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly NotifyBotContext context;

        public UserRepository(UserManager<AppUser> userManager,
            JWTService jwtService,
            SignInManager<AppUser> signInManager,
            NotifyBotContext context)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.signInManager = signInManager;
            this.context = context;
        }

        public async Task<IdentityResult> CreateUser(AppUser user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, SeedData.UserRole);
            return result;
        }
        public async Task<AppUser> GetUserByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }
        public async Task<AppUser> GetUserById(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user;
        }
        public ICollection<AppUser> GetUsers(int pageIndex, int pageSize)
        {
            var users = userManager.Users
                .Where(u => u.Email != SeedData.AdminEmail)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
            return users;
        }
        public ICollection<AppUser> GetUsersByName(string searchString, int pageIndex, int pageSize)
        {
            var searchName = searchString.Trim().ToLower();
            var query = from u in userManager.Users
                        where u.FirstName.ToLower().Contains(searchName) || u.LastName.ToLower().Contains(searchName)
                        select u;
            var users = query
                .Where(u => u.Email != SeedData.AdminEmail)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToList();
            return users;
        }
        public async Task<bool> UserExistedByEmail(string email)
        {
            return await userManager.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<UserDto> CreateApplicationUserDto(AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await jwtService.CreateJWT(user)
            };
        }
        public async Task<IdentityResult> DeleteUser(AppUser user)
        {
            return await userManager.DeleteAsync(user);
        }
        public async Task<SignInResult> CheckPasswordSign(AppUser user, string password)
        {
            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            return result;
        }
        public async Task IncreaseAccessFailed(AppUser user)
        {
            await userManager.AccessFailedAsync(user);
        }
        public async Task<bool> IsLockedOut(AppUser user)
        {
            return await userManager.IsLockedOutAsync(user);
        }
        public async Task<string> GenerateEmailConfirmationToken(AppUser user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<IdentityResult> ConfirmEmail(AppUser user, string token)
        {
            var result = await userManager.ConfirmEmailAsync(user, token);
            return result;
        }
        public async Task<string> GeneratePasswordResetToken(AppUser user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<IdentityResult> ResetPassword(AppUser user, string token, string password)
        {
            var result = await userManager.ResetPasswordAsync(user, token, password);
            return result;
        }
        public long CountUsers()
        {
            var count = context.Users.Count();
            return context.Users.Count();
        }
        public long CountUserSearchs(string searchString)
        {
            var searchName = searchString.Trim().ToLower();
            var query = from u in userManager.Users
                        where u.FirstName.ToLower().Contains(searchName) || u.LastName.ToLower().Contains(searchName)
                        select u;
            return query.Count();
        }
        public async Task<IdentityResult> LockUser(AppUser user, int day)
        {
            var result = await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            return result;
        }
        public async Task<IdentityResult> UnlockedOut(AppUser user)
        {
            var result = await userManager.SetLockoutEndDateAsync(user, null);
            return result;
        }
        public async Task<bool> DeleteAllUser()
        {
            var allUsers = context.Users.Where(u => u.Email != SeedData.AdminEmail);
            context.Users.RemoveRange(allUsers);
            if(allUsers.Any() == false)
            {
                return true;
            }
            var r  = await context.SaveChangesAsync();
            return r > 0;
        }
        public async Task<bool> SeedUsers(int numberOfUsers, string password)
        {
            Faker<AppUser> fakerUser = new Faker<AppUser>();
            fakerUser.RuleFor(s => s.FirstName, f => f.Name.FirstName());
            fakerUser.RuleFor(s => s.LastName, f => f.Name.LastName());
            fakerUser.RuleFor(s => s.DateCreated, f => f.Date.Past(1));
            fakerUser.RuleFor(s => s.Email, f => f.Internet.Email());


            
            for(int i = 1; i <= numberOfUsers; i++)
            {
                AppUser user = fakerUser.Generate();
                user.UserName = user.Email;
                user.EmailConfirmed = true;
                var result = await CreateUser(user, password);
                if(result.Succeeded == false)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<string> GetRolesUser(AppUser user)
        {
            var listRoles = await userManager
                .GetRolesAsync(user);
;           var rolesString = string.Join(",", listRoles);
            return rolesString;
        }
        public async Task<IdentityResult> SetRoleUser(AppUser user, string[] newRoles)
        {
            var roleString = await GetRolesUser(user);
            var roles = roleString.Split(',');
            await userManager.RemoveFromRolesAsync(user, roles);
            var result = await userManager.AddToRolesAsync(user, newRoles);
            return result;
        }
        
        
        public async Task<IdentityResult> UpdateUser(AppUser user, UpdateUserDto model)
        {
            var _user = await GetUserByEmail(user.Email);

            _user.FirstName = model.FirstName;
            _user.LastName = model.LastName;

            var resultUpdate = await userManager.UpdateAsync(_user);
            return resultUpdate;
        }
        public async Task<IdentityResult> ChangePassword(AppUser user, string oldPassword, string newPassword)
        {
            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result;
        }

        public async Task<IdentityResult> UpdateConnectionId(string userId, string connectionId)
        {
            var user = await GetUserById(userId);
            user.ConnectionId = connectionId;
            var resultUpdate = await userManager.UpdateAsync(user);
            return resultUpdate;
        }
    }
}
