using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Admin
{
    public class UserToEditDto
    {
        public string Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [Required]
        public string ReEnterNewPassword { get; set; }
    }
}
