using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifyBotApi.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public bool HasNewMessage { get; set; }

        public ICollection<GroupUser> GroupUsers { get; set; }

        public ICollection<MessageChat> MessageChats { get; set; }
    }
}
