using System.ComponentModel.DataAnnotations.Schema;

namespace NotifyBotApi.Models
{
    [Table("GroupUsers")]
    public class GroupUser
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string GroupId { get; set; }
        public Group Group { get; set; }
    }
}
