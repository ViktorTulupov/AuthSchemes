using AuthenticationService.Models;
using System.Collections.Generic;

namespace AuthenticationService.Contexts
{
    public class UserContext
    {
        public IEnumerable<User> Users
        {
            get
            {
                return new List<User>
                {
                    new User
                    {
                        Id = System.Guid.NewGuid(),
                        Login = "User",
                        Password = "XXXxloJGEk2KjFz28cMXvg==",
                        Role = "User"

                    },
                    new User
                    {
                        Id = System.Guid.NewGuid(),
                        Login = "Admin",
                        Password = "fSNlrz2ZHEmZdAB4xMQS0Q==",
                        Role = "Admin"
                    }
                };
            }
        }
    }
}
