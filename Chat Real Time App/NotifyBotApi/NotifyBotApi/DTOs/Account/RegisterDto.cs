using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Pasword must be at least {2} and max length is {1} characters")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password is not match")]
        public string ReEnterPassword { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string LastName { get; set; }
    }
}
