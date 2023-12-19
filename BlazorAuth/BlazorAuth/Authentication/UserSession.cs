namespace BlazorAuth.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); // Ensure it's initialized
    }
}