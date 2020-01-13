using AuthService.Models;

namespace AuthService.Services
{
    public interface IAuthenticationService
    {
        User BasicAuthenticate(string userName, string password);
        User HMACAuthenticate(string userName, string timeStamp, string hash);
        User BearerAuthenticate(string token);
    }
}
