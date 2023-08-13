using AuthApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NotifyBotApi.DTOs.Account;
using NotifyBotApi.DTOs.EmailSender;
using NotifyBotApi.Interfaces;
using NotifyBotApi.Models;
using NotifyBotApi.Models.MailService;
using NotifyBotApi.Services;
using System.Security.Claims;
using System.Text;

namespace NotifyBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ResultErrorsObj resultError;
        private readonly IConfiguration configuration; // lấy cấu hình gửi mail
        private readonly IEmailSender emailSender; // thực hiện gửi mail

        public AccountController(
            IUserRepository userRepository,
            ResultErrorsObj resultErrors,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            this.userRepository = userRepository;
            this.resultError = resultErrors;
            this.configuration = configuration;
            this.emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Regiser(RegisterDto model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }

            if (await userRepository.UserExistedByEmail(model.Email))
            {
                return BadRequest("This email has been registered. Please use another email.");
            }

            if (model.LastName.Trim().ToLower() == "admin" || model.FirstName.Trim().ToLower() == "admin")
            {
                return BadRequest("FirstName or LastName is invalid.");
            }

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                DateCreated = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var result = await userRepository.CreateUser(user, model.Password);
            if (result.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(result.Errors));
            }

            if (await SendEmailConfirmAsync(user) == true)
            {
                return Ok(new JsonResult(new { title = "Verify Your Email", message = "Please check your email to confirm email to verify your account." }));
            }

            return BadRequest(new { title = "Registration error ", message = "Please re-register" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            if (model == null)
            {
                return BadRequest("Email or password is incorrect");
            }

            var user = await userRepository.GetUserByEmail(model.Email);

            if (user == null)
            {
                return BadRequest("Email or password is incorrect");
            }

            if (user.EmailConfirmed == false)
            {
                return Unauthorized("Please confirm your email");
            }

            var result = await userRepository.CheckPasswordSign(user, model.Password);

            if (result.IsLockedOut)
            {
                return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time)" +
                    "to be able to login", user.LockoutEnd));
            }

            if (result.Succeeded == false)
            {
                if (user.UserName.Equals("admin@example.com") == false)
                {
                    /*
                     * tăng số lần đăng nhập sai
                     */
                    await userRepository.IncreaseAccessFailed(user);
                }

                return BadRequest("Email or password is incorrect");
            }

            return await userRepository.CreateApplicationUserDto(user);
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await userRepository.GetUserByEmail(User.FindFirst(ClaimTypes.Email)?.Value!);
            if (await userRepository.IsLockedOut(user))
            {
                return Unauthorized("You have been lock out");
            }

            return await userRepository.CreateApplicationUserDto(user!);
        }

        [HttpPost("send-mail")]
        public IActionResult SendMail(MessageDto messageDto)
        {
            if (messageDto.To == null || messageDto.To.Count() == 0)
            {
                ModelState.AddModelError("Error", "Danh sách người nhận không được bỏ trống");
                return BadRequest(ModelState);
            }

            Message message = new Message(messageDto.To, messageDto.Subject, messageDto.Content);
            emailSender.SendEmail(message);
            return Ok("Gửi thành công.");
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirm)
        {
            var user = await userRepository.GetUserByEmail(confirm.email);

            if (user == null)
            {
                return BadRequest(new JsonResult(new { title = "Error", message = "This email has not been registered." }));
            }

            if (user.EmailConfirmed == true)
            {
                return BadRequest(new JsonResult(new { title = "Email confirmed", message = "Your email was confirm before. Please login your account" }));
            }


            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(confirm.token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await userRepository.ConfirmEmail(user, decodedToken);

                if (result.Succeeded == true)
                {
                    return Ok(new JsonResult(new { title = "Email confirmed", message = "Your email addres is comfirmed. You can login now." }));
                }
                return BadRequest(new JsonResult(new { title = "Error", message = "Invalid token. Try again." }));
            }
            catch (Exception)
            {
                return BadRequest(new JsonResult(new { title = "Error", message = "Invalid token. Try again." }));
            }
        }

        [HttpPost("resend-confirmation-email/{email}")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Incorrect Email or Password");
            }

            var user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return BadRequest("Incorrect Email or Password");
            }

            if (user.EmailConfirmed == true)
            {
                return BadRequest("Your email is conformed before. You can login now.");
            }

            try
            {
                if (await SendEmailConfirmAsync(user))
                {
                    return Ok(new JsonResult(new { title = "Verify your Email", message = "Please check your email to confirm email to verify your account." }));
                }
                return BadRequest("Fail to send email confirmation. Please try again.");
            }
            catch (Exception)
            {
                return BadRequest("Fail to send email confirmation. Please try again.");
            }

        }

        [HttpPost("forgot-password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is incorrect");
            }

            var user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return BadRequest("Email is incorrect");
            }

            try
            {
                if (await SendForgotPasswordEmail(user))
                {
                    return Ok(new JsonResult(new { title = "Forgot password email sent", message = "Please check your email." }));
                }
                return BadRequest("Failed to sent email. Please contact again.");
            }
            catch (Exception)
            {
                return BadRequest("Failed to sent email. Please contact again.");
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }
            var user = await userRepository.GetUserByEmail(model.Email);
            if (user == null)
            {
                return BadRequest("Email is incorrect");
            }

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await userRepository.ResetPassword(user, decodedToken, model.Password);

                if (result.Succeeded == true)
                {
                    return Ok(new JsonResult(new
                    {
                        title = "Password has been reseted",
                        message = "Your password is reseted. You can login now."
                    }));
                }
                return BadRequest("Invalid token. Try again.");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Try again.");
            }
        }

        [HttpGet("get-user/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ModelState);
            }

            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            UpdateUserDto model = new UpdateUserDto
            {
                Email = user.Email,
                OldPassword = "",
                NewPassword = "",
                ReEnterPassword = "",
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            return Ok(model);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }

            if (model.LastName.Trim().ToLower() == "admin" || model.FirstName.Trim().ToLower() == "admin")
            {
                return BadRequest("FirstName or LastName is invalid.");
            }

            var user = await userRepository.GetUserByEmail(model.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // kiểm tra người dùng có đối nhập mật khẩu cũ để đổi mk mới không
            if (string.IsNullOrEmpty(model.OldPassword) == false ||
                string.IsNullOrEmpty(model.NewPassword) == false ||
                string.IsNullOrEmpty(model.ReEnterPassword) == false)
            {

                // check old password
                var resultCheckPassword = await userRepository.CheckPasswordSign(user, model.OldPassword);
                if (resultCheckPassword.Succeeded == false)
                {
                    return BadRequest("Incorrect old password");
                }

                if (model.NewPassword.Length < 6 || model.NewPassword.Length > 16)
                {
                    return BadRequest("New password must be at least 6 and max length is 16 characters.");
                }
            }

            var resultUpdate = await userRepository.UpdateUser(user, model);
            if (resultUpdate.Succeeded == false)
            {
                return BadRequest(resultError.ToErrorObj(resultUpdate.Errors));
            }

            if (string.IsNullOrEmpty(model.NewPassword) == false)
            {
                var resultChangePassword = await userRepository
                    .ChangePassword(user, model.OldPassword, model.NewPassword);

                if (resultChangePassword.Succeeded == false)
                {
                    return BadRequest(resultError.ToErrorObj(resultChangePassword.Errors));
                }
            }
            return Ok(new JsonResult(new
            {
                title = "Update profile successfully",
                message = "Your profile has been updated."
            }));
        }

        // ==============================================================================================

        #region private methods
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

        private async Task<bool> SendForgotPasswordEmail(AppUser user)
        {
            var token = await userRepository.GeneratePasswordResetToken(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{configuration["JWT:UrlClient"]}/" +
                $"{configuration["EmailConfiguration:ResetPasswordPath"]}" +
                $"?token={token}&email={user.Email}";

            Message message = new Message(new string[] { user.Email! },
                "Reset password",
                $"<p>To reset your password, please click <a href='{url}'>here</a></p>"!);
            return await emailSender.SendEmail(message);
        }

        #endregion
    }
}
