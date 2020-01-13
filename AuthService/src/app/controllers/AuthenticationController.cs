using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Authentication;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("basic")]
        public IActionResult BasicAuthenticate([FromBody]AuthParams authParams)
        {
            if (string.IsNullOrWhiteSpace(authParams.UserName) || string.IsNullOrWhiteSpace(authParams.Password))
            {
                var result = _authenticationService.BasicAuthenticate(authParams.UserName, authParams.Password);
                return Ok(result);
            }
            throw new AuthenticationException("UserName and Password parameters can not be null.");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("hmac")]
        public IActionResult HmacAuthenticate([FromBody]AuthParams authParams)
        {
            if (!string.IsNullOrWhiteSpace(authParams.UserName) || string.IsNullOrWhiteSpace(authParams.Hash) || string.IsNullOrWhiteSpace(authParams.TimeStamp))
            {
                var result = _authenticationService.HMACAuthenticate(authParams.UserName, authParams.TimeStamp, authParams.Hash);
                return Ok(result);
            }
            throw new AuthenticationException("UserName, Hash, TimeStamp parameters can not be null.");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("bearer")]
        public IActionResult BearerAuthenticate([FromBody]AuthParams authParams)
        {
            if (!string.IsNullOrWhiteSpace(authParams.Token))
            {
                var result = _authenticationService.BearerAuthenticate(authParams.Token);
                return Ok(result);
            }
            throw new AuthenticationException("Token parameter can not be null.");
        }
    }
}
