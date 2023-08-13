namespace NotifyBotApi.DTOs.Admin
{
    public class UserView
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsLockout { get; set; }
        public string roles { get; set; }
    }
}
