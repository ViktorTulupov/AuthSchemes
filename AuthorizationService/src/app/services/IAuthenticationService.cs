using AuthorizationService.Models;
using System.Threading.Tasks;

namespace AuthorizationService.Services
{
    public interface IAuthenticationService
    {
        Task<User> Authentication(AuthenticationOptions options, AuthenticationParameters parameters);
    }
}
