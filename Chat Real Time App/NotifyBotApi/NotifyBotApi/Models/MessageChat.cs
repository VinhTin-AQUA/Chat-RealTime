using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifyBotApi.Models
{
    [Table("MessageChats")]
    public class MessageChat
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Sender { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DateSend { get; set; }

        public string GroupId { get; set; }
        public Group Group { get; set; }
    }
}
