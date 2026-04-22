using Microsoft.AspNetCore.Mvc;
using webResfulAPIs.Models;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {

        private AppDbContext appDbContext;
        public AuthenticationController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {

            return Ok();
        }
    }
}
