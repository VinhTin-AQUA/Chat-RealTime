using Microsoft.AspNetCore.Identity;

namespace NotifyBotApi.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime DateCreated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConnectionId { get; set; }

        public ICollection<GroupUser> GroupUsers { get; set; }

    }
}
