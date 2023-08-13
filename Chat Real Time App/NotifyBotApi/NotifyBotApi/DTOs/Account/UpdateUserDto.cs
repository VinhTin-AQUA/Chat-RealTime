using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NotifyBotApi.DTOs.Account
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Password is not match")]
        public string ReEnterPassword { get; set; }
    }
}
