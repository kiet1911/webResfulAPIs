using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Models;
using webResfulAPIs.Models.DTO;

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

                var boardGame = await appDbContext.BoardGames.Where(t => t.Id == guid).Include(bg => bg.BoardGameCreators).ThenInclude(bgc => bgc.Creator).Include(bgc=>bgc.BoardGameImages).Select(
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
                        Categories = bg.BoardGameCategories.Select(bg => new { bg.Category.Id, bg.Category.Name }).ToList(),
                        Description = description,
                        Images = bg.BoardGameImages.Select(bgc => new { bgc.Id, bgc.Alt, bgc.Img_Url, bgc.Is_Thumbnail }).ToList()
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
                var boardGames = await appDbContext.BoardGames.OrderByDescending(t => t.Sold_Quantity).Take(20).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category).Include(t=>t.BoardGameImages)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList(),
                   Images = bg.BoardGameImages.Select(bgc => new {bgc.Id,bgc.Alt,bgc.Img_Url,bgc.Is_Thumbnail}).ToList()
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
                var boardGames = await appDbContext.BoardGames.OrderByDescending(t => t.Created_at).Take(20).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category).Include(t => t.BoardGameImages)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList(),
                   Images = bg.BoardGameImages.Select(bgc => new { bgc.Id, bgc.Alt, bgc.Img_Url, bgc.Is_Thumbnail }).ToList()
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
                var boardGames = await appDbContext.BoardGames.OrderByDescending(t => t.Rating).Take(20).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category).Include(t => t.BoardGameImages)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList(),
                   Images = bg.BoardGameImages.Select(bgc => new { bgc.Id, bgc.Alt, bgc.Img_Url, bgc.Is_Thumbnail }).ToList()
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
        public async Task<IActionResult> Boardgames()
        {
            try
            {
                var boardGames = await appDbContext.BoardGames.Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category).Include(t => t.BoardGameImages)
               .Select(bg => new
               {
                   bg.Id,
                   bg.Name,
                   bg.Base_Price,
                   bg.Sold_Quantity,
                   bg.Created_at,
                   bg.Rating,
                   Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList(),
                   Images = bg.BoardGameImages.Select(bgc => new { bgc.Id, bgc.Alt, bgc.Img_Url, bgc.Is_Thumbnail }).ToList()
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
        [HttpPost]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "Price", "Playtime", "Rating", "Complexity", "Age", "Page", "PageSize" })]
        public async Task<IActionResult> BoardGamesFilter([FromQuery] BoardGamesFilter Filters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("error data not full fill");
            }
            //begin query
            try
            {
                var skiCount = Filters.Page * Filters.PageSize;

                var gameFilter = await appDbContext.BoardGames.OrderBy(t => t.Id).Where(q => q.Base_Price <= Filters.Price && q.Max_Time <= Filters.Playtime && q.Rating <= Filters.Rating && q.Complexity <= Filters.Complexity && q.Age_Requirement <= Filters.Age).Skip(skiCount).Take(Filters.PageSize).Include(bg => bg.BoardGameCategories).ThenInclude(bgc => bgc.Category).Include(t => t.BoardGameImages).Select(bg => new
                {
                    bg.Id,
                    bg.Name,
                    bg.Base_Price,
                    bg.Sold_Quantity,
                    bg.Created_at,
                    bg.Rating,
                    Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList(),
                    Images = bg.BoardGameImages.Select(bgc => new { bgc.Id, bgc.Alt, bgc.Img_Url, bgc.Is_Thumbnail }).ToList()
                }).ToListAsync();
                return Ok(new
                {
                    gameLists = gameFilter,
                    page = gameFilter?.Count == 0 ? (Filters.Page) : Filters.Page,
                    isMax = gameFilter?.Count == 0 ? true : false,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error"
                });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "Search", "Page", "PageSize" })]
        public async Task<IActionResult> BoardGamesSearch([FromQuery] BoardGamesSearch searchfilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("error data not full fill");
            }
            //begin query
            try
            {
                var skiCount = searchfilter.Page * searchfilter.PageSize;
                var gameFilter = await appDbContext.BoardGames.OrderBy(t => t.Id).Where(t => t.Name.Contains(searchfilter.Search.Trim())).Skip(skiCount).Take(searchfilter.PageSize).Select(bg => new
                {
                    bg.Id,
                    bg.Name,
                    bg.Base_Price,
                    bg.Sold_Quantity,
                    bg.Created_at,
                    bg.Rating,
                    Categories = bg.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList()
                }).ToListAsync();
                return Ok(new
                {
                    gameLists = gameFilter,
                    page = gameFilter?.Count == 0 ? (searchfilter.Page) : searchfilter.Page,
                    isMax = gameFilter?.Count == 0 ? true : false,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error"
                });
            }
        }
    }
}
