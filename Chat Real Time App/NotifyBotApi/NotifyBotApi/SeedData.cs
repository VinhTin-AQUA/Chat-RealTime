namespace NotifyBotApi
{
    public static class SeedData
    {
        // roles
        public static string AdminRole = "Admin";
        public static string ManagerRole = "Manager";
        public static string UserRole = "User";

        // admin
        public static string AdminEmail { get; } = "admin@example.com";
        public static string AdminFirstName { get; } = "Admin";
        public static string AdminLastName { get; } = "Admin";
        public static string AdminPassword { get; } = "admin";

    }
}
