using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthorizationService.Handlers
{
    public class BearerAuthenticationHandler : AuthenticationHandler<Models.AuthenticationOptions>
    {
        private readonly Services.IAuthenticationService _authenticationService;

        public BearerAuthenticationHandler(
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

                if (!Enums.AuthenticationSchemes.Bearer.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(headerValue.Parameter))
                {
                    return AuthenticateResult.NoResult();
                }

                var authParams = new Models.AuthenticationParameters
                {
                    Token = headerValue.Parameter
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
