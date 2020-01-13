namespace AuthService.Models
{
    public class AuthParams
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string TimeStamp { get; set; }

        public string Hash { get; set; }

        public string Token { get; set; }
    }
}
