using System.ComponentModel.DataAnnotations;

namespace NotifyBotApi.DTOs.Chat
{
    public class MessageChatToSend
    {
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
