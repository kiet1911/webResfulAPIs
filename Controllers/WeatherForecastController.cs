using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using webResfulAPIs.Helpers.Tokens;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        [Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult demoToken()
        {
            //cookies options 
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(7),
                MaxAge = TimeSpan.FromDays(7),
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };
            //access
            var accessToken = GenerateTokens.GenerateAccessTokens(_configuration);
            var refreshToken = GenerateTokens.GenerateRefreshTokens();
            var jsonToken = new { accessToken = accessToken , refreshToken = refreshToken };
            Response.Cookies.Append("accessToken", accessToken , options);
            //refresh 
            return Ok(jsonToken);
        }
    }
}
