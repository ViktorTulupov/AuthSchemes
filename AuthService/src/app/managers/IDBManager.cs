using AuthService.Models;
using System.Collections.Generic;

namespace AuthService.Managers
{
    public interface IDBManager
    {
        List<User> GetUsers();
    }
}
