using AuthenticationService.Models;
using System;
using System.Collections.Generic;

namespace AuthenticationService.Managers
{
    public class DBManager : IDBManager
    {
        //private readonly string _connectionString;

        //public DBManager(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        //private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Login = "User",
                    Password = Guid.NewGuid().ToString(),
                    Role = "User"

                },
                new User
                {
                    Login = "Admin",
                    Password = Guid.NewGuid().ToString(),
                    Role = "Admin"
                }
            };
        }
    }
}
