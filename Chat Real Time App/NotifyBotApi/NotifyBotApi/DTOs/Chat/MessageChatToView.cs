

namespace NotifyBotApi.DTOs.Chat
{
    public class MessageChatToView
    {
        public string Id { get; set; }

        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime DateSend { get; set; }
    }
}
