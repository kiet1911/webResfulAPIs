using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.ObjectHelper;
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
            //Console.WriteLine(userDetailToken.Value);
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Id([FromBody] string publicId)
        {
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            if (publicIdCookies == null || publicId != publicIdCookies.Value) return StatusCode(StatusCodes.Status403Forbidden,
              new ResponseConfigure().CustomResponse("error", "Wrong user information", "403")
            );

            //EF query
            try
            {
                var useWithFavorits = await appDbContext.Users.Where(t => t.Public_id == publicId).Include(t => t.Favorites).ThenInclude(t => t.BoardGame).ThenInclude(t => t.BoardGameCategories).ThenInclude(t => t.Category).AsNoTracking().FirstOrDefaultAsync();

                if(useWithFavorits == null)
                {
                    return NotFound(new ResponseConfigure().CustomResponse("error", "User not found", "404"));
                }

                var favorites = useWithFavorits.Favorites.Select(bg => new
                {
                    bg.BoardGame.Id,
                    bg.BoardGame.Name,
                    bg.BoardGame.Base_Price,
                    bg.BoardGame.Sold_Quantity,
                    bg.BoardGame.Created_at,
                    bg.BoardGame.Rating,
                    Categories = bg.BoardGame.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList()
                });

                return Ok(new
                {
                    status = 200,
                    //favorites = lstFavorits.Select(t => new { boardGameId = t.Favorites.Select(t=> new {t.BoardGame_Id}) })
                    favorites = favorites
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }
        }

    }
}
