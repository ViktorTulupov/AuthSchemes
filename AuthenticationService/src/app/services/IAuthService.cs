using AuthenticationService.Models;

namespace AuthenticationService.Services
{
    public interface IAuthService
    {
        User BasicAuthenticate(string userName, string password);

        User HMACAuthenticate(string userName, string timeStamp, string hash);

        string HashGenerate(string login);

        User BearerAuthenticate(string token);

        string TokenGenerate(string login);
    }
}
