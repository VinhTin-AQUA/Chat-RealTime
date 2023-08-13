using Microsoft.AspNetCore.Identity;
using NotifyBotApi.DTOs.Account;
using NotifyBotApi.Models;
using System.Threading.Tasks;

namespace NotifyBotApi.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByEmail(string email);
        Task<AppUser> GetUserById(string userId);
        ICollection<AppUser> GetUsers(int pageIndex, int pageSize);
        ICollection<AppUser> GetUsersByName(string searchString, int pageIndex, int pageSize);
        long CountUsers();
        long CountUserSearchs(string searchString);
        Task<bool> UserExistedByEmail(string email);
        Task<IdentityResult> CreateUser(AppUser user, string password);
        Task<IdentityResult> DeleteUser(AppUser user);
        Task<UserDto> CreateApplicationUserDto(AppUser user);
        Task<SignInResult> CheckPasswordSign(AppUser user, string password);
        Task IncreaseAccessFailed(AppUser user);
        Task<bool> IsLockedOut(AppUser user);
        Task<string> GenerateEmailConfirmationToken(AppUser user);
        Task<string> GeneratePasswordResetToken(AppUser user);
        Task<IdentityResult> ConfirmEmail(AppUser user, string token);
        Task<IdentityResult> ResetPassword(AppUser user, string token, string password);
        Task<IdentityResult> LockUser(AppUser user, int day);
        Task<IdentityResult> UnlockedOut(AppUser user);
        Task<bool> DeleteAllUser();
        Task<bool> SeedUsers(int numberOfUsers, string password);
        Task<string> GetRolesUser(AppUser user);
        Task<IdentityResult> SetRoleUser(AppUser user, string[] newRoles);

        Task<IdentityResult> UpdateUser(AppUser user, UpdateUserDto model);
        Task<IdentityResult> ChangePassword(AppUser user, string oldPassword, string newPassword);

        Task<IdentityResult> UpdateConnectionId(string userId, string connectionId);
    }
}
