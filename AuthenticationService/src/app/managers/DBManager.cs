using AuthenticationService.Models;
using System;
using System.Collections.Generic;

namespace AuthenticationService.Managers
{
    public class DBManager : IDBManager
    {
        public List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Login = "User",
                    Password = "XXXxloJGEk2KjFz28cMXvg==",
                    Role = "User"

                },
                new User
                {
                    Login = "Admin",
                    Password = "fSNlrz2ZHEmZdAB4xMQS0Q==",
                    Role = "Admin"
                }
            };
        }
    }
}
