using System.Collections.Generic;

namespace AuthenticationService.Models
{
    public class User
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
