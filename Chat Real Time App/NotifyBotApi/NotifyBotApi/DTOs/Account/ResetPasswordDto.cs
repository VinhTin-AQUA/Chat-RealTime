using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Pasword must be at least {2} and max length is {1} characters")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Confirm password is not match")]
        public string ConfirmPassword { get; set;
        }
    }
}
