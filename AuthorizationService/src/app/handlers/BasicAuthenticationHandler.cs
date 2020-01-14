using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthorizationService.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<Models.AuthenticationOptions>
    {
        private readonly Services.IAuthenticationService _authenticationService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<Models.AuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            Services.IAuthenticationService authenticationService)
            : base(options, loggerFactory, encoder, clock)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            Response.ContentType = "text/plain";
            Response.WriteAsync("Authorization error");
            return Task.CompletedTask;
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

                if (!Enums.AuthenticationSchemes.Basic.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.NoResult();
                }

                byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
                string userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
                string[] parts = userAndPassword.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid Basic authentication header");
                }

                var authParams = new Models.AuthenticationParameters
                {
                    Login = parts[0],
                    Password = parts[1]
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

                return AuthenticateResult.Fail("Invalid login or password");
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(e.Message);
            }
        }
    }
}

