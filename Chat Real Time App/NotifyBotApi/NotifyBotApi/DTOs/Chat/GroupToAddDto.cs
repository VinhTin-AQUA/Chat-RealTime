using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Chat
{
    public class GroupToAddDto
    {
        [Required]
        public string Name { get; set; }
    }
}
