using AuthenticationService.Models;
using System.Collections.Generic;

namespace AuthenticationService.Managers
{
    public interface IDBManager
    {
        List<User> GetUsers();
    }
}
