using AuthService.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace AuthService.Managers
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
                    Name = "test",
                    Password = "12345",
                    Roles = {
                        new Role
                        {
                            Name = "user"
                        }
                    }
                }
            };
        }
    }
}
