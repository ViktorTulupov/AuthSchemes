using System;
using System.Collections.Generic;
using System.Linq;
using AuthorizationService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.Controllers
{
    [Authorize(AuthenticationSchemes = authenticationSchemes)]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public const string authenticationSchemes = Enums.AuthenticationSchemes.Basic + "," + Enums.AuthenticationSchemes.HMAC + "," + Enums.AuthenticationSchemes.Bearer;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}
