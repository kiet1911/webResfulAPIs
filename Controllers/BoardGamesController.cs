using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.EnumsStore;
using webResfulAPIs.Models;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class BoardGamesController : ControllerBase
    {

        private AppDbContext appDbContext;
        private IConfiguration configuration;
        public BoardGamesController(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Id([FromQuery] Guid guid)
        {
            Console.WriteLine("this is guid {0}", guid);
            try
            {
                var description = await appDbContext.BoardGameDescriptions.Where(t => t.BoardGame_Id == guid).FirstOrDefaultAsync();

                var boardGame = await appDbContext.BoardGames.Where(t => t.Id == guid).Include(bg => bg.BoardGameCreators).ThenInclude(bgc => bgc.Creator).Select(
                    bg => new
                    {
                        bg.Id,
                        bg.Name,
                        bg.Base_Price,
                        bg.Stock_Quantity,
                        bg.Status,
                        bg.Weight,
                        bg.Size_X,
                        bg.Size_Y,
                        bg.Size_Z,
                        bg.Min_Player,
                        bg.Max_Player,
                        bg.Min_Time,
                        bg.Max_Time,
                        bg.Prefer_Player,
                        bg.Complexity,
                        bg.Rating,
                        bg.Age_Requirement,
                        Creators = bg.BoardGameCreators.Select(bg => new { bg.Creator.Id, bg.Creator.Name, bg.Creator.Type }).ToList(),
                        Categories = bg.BoardGameCategories.Select(bg=> new { bg.Category.Id , bg.Category.Name }).ToList(),
                        Description = description
                    }
                    ).FirstOrDefaultAsync();
                return Ok(boardGame);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> BestSeller()
        {
            try
            {
                var boardGames = await appDbContext.BoardGames.OrderByDescending(t => t.Sold_Quantity).Take(20).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList()
               })
               .ToListAsync();
                return Ok(boardGames);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> NewReleasedGame()
        {
            try
            {
                var boardGames = await appDbContext.BoardGames.OrderByDescending(t => t.Created_at).Take(20).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList()
               })
               .ToListAsync();
                return Ok(boardGames);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RatingGame()
        {
            try
            {
                var boardGames = await appDbContext.BoardGames.OrderByDescending(t => t.Rating).Take(20).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList()
               })
               .ToListAsync();
                return Ok(boardGames);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });
            }
        }
    }
}
