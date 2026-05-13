using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
