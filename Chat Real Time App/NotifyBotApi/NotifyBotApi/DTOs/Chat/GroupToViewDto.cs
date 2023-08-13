namespace NotifyBotApi.DTOs.Chat
{
    public class GroupToViewDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public bool HasNewMessage { get; set; }

        public int Memebers { get; set; }
        public int Onlines { get; set; }
    }
}
