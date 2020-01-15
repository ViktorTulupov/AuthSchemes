using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Authentication;

namespace AuthenticationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "basic")]
    public class BasicController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public BasicController(IAuthService authenticationService)
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
            if (!string.IsNullOrWhiteSpace(authParams.Login) && !string.IsNullOrWhiteSpace(authParams.Password))
            {
                var result = _authenticationService.BasicAuthenticate(authParams.Login, authParams.Password);
                return Ok(result);
            }
            throw new AuthenticationException("Login and Password parameters can not be null.");
        }
    }
}
