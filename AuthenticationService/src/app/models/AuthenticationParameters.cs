namespace AuthenticationService.Models
{
    public class AuthenticationParameters
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string TimeStamp { get; set; }

        public string Hash { get; set; }

        public string Token { get; set; }
    }
}
