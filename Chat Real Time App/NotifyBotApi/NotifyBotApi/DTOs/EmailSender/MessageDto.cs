namespace NotifyBotApi.DTOs.EmailSender
{
    public class MessageDto
    {
        // danh sách mail cần gửi đến
        public IEnumerable<string> To { get; set; } = new List<string>();

        // tiêu đề
        public string Subject { get; set; } = "";

        // nội dung
        public string Content { get; set; } = "";

    }
}
