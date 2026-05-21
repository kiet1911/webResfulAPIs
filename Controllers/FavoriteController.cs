using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Models;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class FavoriteController : ControllerBase
    {
        private AppDbContext appDbContext;
        private IConfiguration configuration;

        public FavoriteController(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Guid boardgameId)
        {
            string refreshToken = null;
            Boolean isDelete = false; 
            HttpContext.Request.Cookies.TryGetValue("RefreshToken", out refreshToken);
            var userDetailToken = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            Console.WriteLine(userDetailToken.Value);
            // try catch get if exits 
            try
            {
                //
                var user = await appDbContext.Users.FirstOrDefaultAsync(t => t.Public_id == userDetailToken.Value);
                if (user == null)
                {
                    return BadRequest();
                }
                var fav = await appDbContext.Favorites.Where(t => t.BoardGame_Id == boardgameId && t.User_Id == user.Id).FirstOrDefaultAsync();
                if (fav == null)
                {
                    await appDbContext.Favorites.AddAsync(new Favorites { BoardGame_Id = boardgameId, User_Id = user.Id });
                    await appDbContext.SaveChangesAsync();
                }
                else
                {
                    appDbContext.Favorites.Remove(fav);
                    await appDbContext.SaveChangesAsync();
                    isDelete = true;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new
            {
                status = 200,
                message = "success",
                data = boardgameId,
                publicId = userDetailToken.Value,
                isdelete = isDelete
            });
        }

    }
}
