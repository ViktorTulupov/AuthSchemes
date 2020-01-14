using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthorizationService.Handlers
{
    public class HMACAuthenticationHandler : AuthenticationHandler<Models.AuthenticationOptions>
    {
        private readonly Services.IAuthenticationService _authenticationService;

        public HMACAuthenticationHandler(
            IOptionsMonitor<Models.AuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            Services.IAuthenticationService authenticationService)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
               if (!Request.Headers.ContainsKey("Authorization"))
                {
                    return AuthenticateResult.NoResult();
                }

                if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"],
                    out AuthenticationHeaderValue headerValue))
                {
                    return AuthenticateResult.NoResult();
                }

                if (!Enums.AuthenticationSchemes.HMAC.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.NoResult();
                }

                string[] parts = headerValue.Parameter.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                {
                    return AuthenticateResult.Fail("Invalid HMAC authentication header");
                }

                var authParams = new Models.AuthenticationParameters
                {
                    Login = parts[0],
                    Hash = parts[1],
                    TimeStamp = parts[2]
                };

                var user = await _authenticationService.Authentication(Options, authParams);
                if (user != null)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.Role) };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }

                return AuthenticateResult.Fail("Failed authentication");
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(e.Message);
            }
        }
    }
}
