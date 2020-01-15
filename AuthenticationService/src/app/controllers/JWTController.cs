using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Authentication;

namespace AuthenticationService.Controllers
{
    [Route("bearer")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "bearer")]
    public class JWTController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public JWTController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticationParameters authParams)
        {
            if (!string.IsNullOrWhiteSpace(authParams.Token))
            {
                var result = _authenticationService.BearerAuthenticate(authParams.Token);
                return Ok(result);
            }
            throw new AuthenticationException("Token parameter can not be null.");
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Route("token")]
        public IActionResult TokenGenerate(string login)
        {
            if (!string.IsNullOrWhiteSpace(login))
            {
                var result = _authenticationService.TokenGenerate(login);
                return Ok(result);
            }
            throw new AuthenticationException("Login can not be null.");
        }
    }
}
