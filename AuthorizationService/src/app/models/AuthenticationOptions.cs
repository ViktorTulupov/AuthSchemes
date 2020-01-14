using Microsoft.AspNetCore.Authentication;

namespace AuthorizationService.Models
{
    public class AuthenticationOptions : AuthenticationSchemeOptions
    {
        public string MethodName { get; set; }
    }
}
