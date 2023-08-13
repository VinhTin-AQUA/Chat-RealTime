using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Admin
{
    public class UserToAddDto
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Pasword must be at least {2} and max length is {1} characters")]
        public string Password { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
