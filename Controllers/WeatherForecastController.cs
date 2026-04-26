using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webResfulAPIs.Helpers.Tokens;
using webResfulAPIs.Models;
using BC = BCrypt.Net.BCrypt;
namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {

        private AppDbContext appDbContext;


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration, AppDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            appDbContext = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> test_add()
        {
            //new users 
            Users user = new Users();
            user.Email = "droang02@gmail.com";
            user.Password = BC.HashPassword("this is hash password");

            //new profiles 
            Profiles profiles = new Profiles { 
                Full_Name = "Nguyễn Văn",
                Display_Name = "A",
                User = user
            };
            try
            {
                appDbContext.Profiles.Add(profiles);
                await appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
            return Ok(new
            {
                status = 200,
                message = "create success!"
            });
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
            var jsonToken = new { accessToken = accessToken, refreshToken = refreshToken };
            Response.Cookies.Append("accessToken", accessToken, options);
            //refresh 
            return Ok(jsonToken);
        }
    }
}
