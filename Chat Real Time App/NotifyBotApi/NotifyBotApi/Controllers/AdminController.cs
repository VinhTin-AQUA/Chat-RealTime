using AuthApi.Interfaces;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualBasic;
using NotifyBotApi.DTOs.Admin;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using NotifyBotApi.Models.MailService;
using NotifyBotApi.Repositories;
using NotifyBotApi.Services;
using System.Text;

namespace NotifyBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ResultErrorsObj resultError;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly IRoleRepository roleRepository;

        public AdminController(
            IUserRepository userRepository,
            ResultErrorsObj resultError,
            IEmailSender emailSender,
            IConfiguration configuration,
            IRoleRepository roleRepository)
        {
            this.userRepository = userRepository;
            this.resultError = resultError;
            this.emailSender = emailSender;
            this.configuration = configuration;
            this.roleRepository = roleRepository;
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (userId == null)
            {
                return BadRequest(ModelState);
            }

            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest(ModelState);
            }

            var result = await userRepository.DeleteUser(user);
            if (result.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(result.Errors));
            }
            return Ok(new JsonResult(
                new
                {
                    title = $"Delete {user.FirstName} {user.LastName} successfully",
                    message = "This user has been deleted."
                }
            ));
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            int index = pageIndex * pageSize;
            var users = userRepository.GetUsers(pageIndex, pageSize).ToList();
            var usersResult = users.Where(u => u.Email != "admin@example.com")
                .Select(u => new UserView
                {
                    Index = ++index,
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateCreated = u.DateCreated,
                }).ToList();
            await updateSomeProp(usersResult);
            return Ok(new { users = usersResult, size = userRepository.CountUsers() });
        }

        [HttpGet("search-users")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetUserByName([FromQuery] string searchString, [FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            int index = pageIndex * pageSize;
            var usersResult = userRepository
                .GetUsersByName(searchString, pageIndex, pageSize)
                .Select(u => new UserView
                {
                    Index = ++index,
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateCreated = u.DateCreated,
                }).ToList();
            await updateSomeProp(usersResult);
            return Ok(new { users = usersResult, size = userRepository.CountUserSearchs(searchString) });
        }

        [HttpGet("count-users")]
        public IActionResult GetCountUsers()
        {
            return Ok();
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(UserToAddDto model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }

            var userExisted = await userRepository.GetUserByEmail(model.Email);
            if (userExisted != null)
            {
                return BadRequest("This email had been registered before.");
            }

            var userToAdd = new AppUser
            {
                FirstName = model.Firstname,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                DateCreated = DateTime.UtcNow
            };

            var result = await userRepository.CreateUser(userToAdd, model.Password);
            if (result.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(result.Errors));
            }

            if (await SendEmailConfirmAsync(userToAdd) == false)
            {
                return BadRequest(string.Format("Something error when send mail confirm to {0}", userToAdd.LastName));
            }

            return Ok(new JsonResult(new { title = "Add user successfully", message = "Don't forget to prompt the user to confirm the email." }));
        }

        [HttpPut("lock-user/{userId}")]
        public async Task<IActionResult> LockUser(string userId, [FromQuery] int day)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ModelState);
            }
            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var result = await userRepository.LockUser(user, day);
            if (result.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(result.Errors));
            }

            return NoContent();
        }

        [HttpPut("unlock-user/{userId}")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ModelState);
            }
            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var result = await userRepository.UnlockedOut(user);
            if (result.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(result.Errors));
            }

            return NoContent();
        }

        [HttpPost("delete-all-users")]
        public async Task<IActionResult> DeleteAllUser()
        {
            var result = await userRepository.DeleteAllUser();
            if (result == false)
            {
                return BadRequest("Something error when delete all user");
            }
            return Ok(new JsonResult(new
            {
                title = "Delete successfully",
                message = "All users have been deleted."
            }));
        }

        [HttpPost("seed-users")]
        public async Task<IActionResult> SeedUser([FromQuery] int numberOfUsers, [FromQuery] string password)
        {
            if (await userRepository.SeedUsers(numberOfUsers, password) == false)
            {
                return BadRequest("Something error when seed users");
            }
            return Ok(new JsonResult(new
            {
                title = "Seed users successfully",
                message = $"Increase {numberOfUsers} users."
            }));
        }

        [HttpGet("get-application-roles")]
        public IActionResult GetApplicationRoles()
        {
            var roles = roleRepository.GetApplicationRoles();
            return Ok(roles);
        }

        [HttpPut("set-roles-user/{userId}")]
        public async Task<IActionResult> SetRoles(string userId, [FromBody] string[] roleStrings)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ModelState);
            }

            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (roleStrings[0] == "")
            {
                return Ok(new JsonResult(new
                {
                    title = "No Update",
                    message = "User roles don't update."
                }));
            }
            var result = await userRepository.SetRoleUser(user, roleStrings);
            if (result.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(result.Errors));
            }
            return Ok(new JsonResult(new
            {
                title = "Update roles successfully",
                message = "User roles has been updated"
            }));
        }

        #region private method
        private async Task<bool> SendEmailConfirmAsync(AppUser user)
        {
            var token = await userRepository.GenerateEmailConfirmationToken(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{configuration["JWT:UrlClient"]}/" +
                $"{configuration["EmailConfiguration:ConfirmationEmailPath"]}" +
                $"?token={token}&email={user.Email}";

            Message message = new Message(new string[] { user.Email! },
                "Confirm Email",
                $"<p>We really happy when you using my app. Click <a href='{url}'>here</a> to verify email</p>"!);
            return await emailSender.SendEmail(message);
        }

        private async Task updateSomeProp(List<UserView> list)
        {
            foreach (var u in list)
            {
                var user = await userRepository.GetUserById(u.Id);
                u.IsLockout = await userRepository.IsLockedOut(user);
                u.roles = await userRepository.GetRolesUser(user);
            }
        }

        #endregion
    }
}
