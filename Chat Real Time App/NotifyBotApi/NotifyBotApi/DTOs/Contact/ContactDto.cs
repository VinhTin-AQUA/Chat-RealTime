using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Contact
{
    public class ContactDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
