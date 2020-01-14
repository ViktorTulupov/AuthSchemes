using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Authentication;

namespace AuthenticationService.Controllers
{
    [Route("authentication")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authenticationService;

        public AuthController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("basic")]
        public IActionResult BasicAuthenticate([FromBody]AuthenticationParameters authParams)
        {
            if (!string.IsNullOrWhiteSpace(authParams.Login) && !string.IsNullOrWhiteSpace(authParams.Password))
            {
                var result = _authenticationService.BasicAuthenticate(authParams.Login, authParams.Password);
                return Ok(result);
            }
            throw new AuthenticationException("Login and Password parameters can not be null.");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("hmac")]
        public IActionResult HmacAuthenticate([FromBody]AuthenticationParameters authParams)
        {
            if (!string.IsNullOrWhiteSpace(authParams.Login) && !string.IsNullOrWhiteSpace(authParams.Hash) && !string.IsNullOrWhiteSpace(authParams.TimeStamp))
            {
                var result = _authenticationService.HMACAuthenticate(authParams.Login, authParams.TimeStamp, authParams.Hash);
                return Ok(result);
            }
            throw new AuthenticationException("Login, Hash, TimeStamp parameters can not be null.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("hmac")]
        public IActionResult HashGenerate(string login)
        {
            if (!string.IsNullOrWhiteSpace(login))
            {
                var result = _authenticationService.HashGenerate(login);
                return Ok(result);
            }
            throw new AuthenticationException("Login can not be null.");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("bearer")]
        public IActionResult BearerAuthenticate([FromBody]AuthenticationParameters authParams)
        {
            if (!string.IsNullOrWhiteSpace(authParams.Token))
            {
                var result = _authenticationService.BearerAuthenticate(authParams.Token);
                return Ok(result);
            }
            throw new AuthenticationException("Token parameter can not be null.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("bearer")]
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
