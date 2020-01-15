using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Authentication;

namespace AuthenticationService.Controllers
{
    [Route("hmac")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "hmac")]
    public class HashController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public HashController(IAuthService authenticationService)
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
            if (!string.IsNullOrWhiteSpace(authParams.Login) && !string.IsNullOrWhiteSpace(authParams.Hash) && !string.IsNullOrWhiteSpace(authParams.TimeStamp))
            {
                var result = _authenticationService.HMACAuthenticate(authParams.Login, authParams.TimeStamp, authParams.Hash);
                return Ok(result);
            }
            throw new AuthenticationException("Login, Hash, TimeStamp parameters can not be null.");
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Route("hash")]
        public IActionResult HashGenerate(string login)
        {
            if (!string.IsNullOrWhiteSpace(login))
            {
                var result = _authenticationService.HashGenerate(login);
                return Ok(result);
            }
            throw new AuthenticationException("Login can not be null.");
        }
    }
}
