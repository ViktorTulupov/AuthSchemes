using System.Collections.Generic;

namespace AuthService.Models
{
    public class User
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public List<Role> Roles { get; set; }
    }
}
