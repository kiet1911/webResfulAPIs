using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.ObjectHelper;
using webResfulAPIs.Models;
using webResfulAPIs.Models.DTO;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class CartController : ControllerBase
    {

        private IConfiguration _configuration;
        private AppDbContext _appdbcontext;

        public CartController(IConfiguration configuration, AppDbContext appDbContext)
        {
            this._configuration = configuration;
            this._appdbcontext = appDbContext;
        }

        //Add or Update cart
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> addOrUpdate([FromBody] CartAddOrUpdate cartAddOrUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "401"));
            }

            //check public id and if exist Cart
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            if (publicIdCookies == null || string.IsNullOrEmpty(cartAddOrUpdate.publicId) || cartAddOrUpdate.publicId != publicIdCookies.Value) return StatusCode(StatusCodes.Status403Forbidden,
              new ResponseConfigure().CustomResponse("error", "User information does not exist", "403")
            );

            try
            {
                //find if user exist 
                var user = await _appdbcontext.Users.FirstOrDefaultAsync(t => t.Public_id == cartAddOrUpdate.publicId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User information does not exist", "403"));
                }
                // check game exist
                var existingGame = await _appdbcontext.BoardGames.FirstOrDefaultAsync(t => t.Id == cartAddOrUpdate.boardgameId);
                if (existingGame == null)
                {
                    return BadRequest(new ResponseConfigure().CustomResponse("error", "boardgame does not exist ", "401"));
                }
                //check cart exist
                var cart = await _appdbcontext.Carts.FirstOrDefaultAsync(t => t.UserId == user.Id);
                var cartUser = new Carts { UserId = user.Id };
                if (cart == null)
                {
                    //create new cart
                    await _appdbcontext.Carts.AddAsync(cartUser);
                    await _appdbcontext.SaveChangesAsync();
                }
                else
                {
                    cartUser = cart;
                }

                //find cart items 
                var cartItems = await _appdbcontext.CartItems.FirstOrDefaultAsync(t => t.CartId == cartUser.Id && t.BoardgameId == existingGame.Id);
                if (cartItems == null)
                {
                    //add if non exist cart items
                    await _appdbcontext.CartItems.AddAsync(new CartItems { CartId = cartUser.Id, BoardgameId = existingGame.Id, Quantity = 1, UnitPrice = existingGame.Base_Price });
                }
                else if (cartItems.Quantity == 1 && !cartAddOrUpdate.isIncrease)
                {
                    _appdbcontext.CartItems.Remove(cartItems);
                    await _appdbcontext.SaveChangesAsync();

                    return Ok(new
                    {
                        status = 200,
                        message = "Remove cart success"
                    });
                }
                else
                {
                    cartItems.Quantity = cartAddOrUpdate.isIncrease ? cartItems.Quantity += 1 : cartItems.Quantity -= 1;
                    _appdbcontext.CartItems.Update(cartItems);
                }

                await _appdbcontext.SaveChangesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Update cart success"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Id([FromBody] string publicId)
        {
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            if (publicIdCookies == null || publicId != publicIdCookies.Value) return StatusCode(StatusCodes.Status403Forbidden,
              new ResponseConfigure().CustomResponse("error", "Wrong user information", "403")
            );
            try
            {

                var useWithCartItems = await _appdbcontext.Users.Where(t => t.Public_id == publicId).FirstOrDefaultAsync();

                if (useWithCartItems == null)
                {
                    return NotFound(new ResponseConfigure().CustomResponse("error", "User not found", "404"));
                }

                var cartItems = await _appdbcontext.Carts.Where(t => t.UserId == useWithCartItems.Id).Include(t => t.CartItems).ThenInclude(t => t.BoardGames).ThenInclude(t => t.BoardGameCategories).ThenInclude(t => t.Category).AsNoTracking().FirstOrDefaultAsync();

                var items = cartItems?.CartItems.Select(bg => new
                {
                    bg.BoardGames.Id,
                    bg.BoardGames.Name,
                    bg.BoardGames.Base_Price,
                    bg.Quantity,
                    Categories = bg.BoardGames.BoardGameCategories.Select(bgc => new { bgc.Category_Id, bgc.Category.Name }).ToList()
                });
                return Ok(new
                {
                    status = 200,
                    cartItems = items
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Id([FromQuery] CartDelete cartDelete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "401"));
            }

            //check public id and if exist Cart
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            if (publicIdCookies == null || string.IsNullOrEmpty(cartDelete.publicId) || cartDelete.publicId != publicIdCookies.Value) return StatusCode(StatusCodes.Status403Forbidden,
              new ResponseConfigure().CustomResponse("error", "User information does not exist", "403")
            );
            try
            {
                //find if user exist 
                var user = await _appdbcontext.Users.FirstOrDefaultAsync(t => t.Public_id == cartDelete.publicId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User information does not exist", "403"));
                }

                //check cart exist
                var cart = await _appdbcontext.Carts.FirstOrDefaultAsync(t => t.UserId == user.Id);
                var cartUser = new Carts { UserId = user.Id };
                if (cart == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "Cart does not exist", "403"));
                }
                else
                {
                    cartUser = cart;
                }

                //find cart items
                var cartItems = await _appdbcontext.CartItems.FirstOrDefaultAsync(t => t.CartId == cartUser.Id && t.BoardgameId == cartDelete.boardgameId);

                if (cartItems != null)
                {
                    _appdbcontext.CartItems.Remove(cartItems);
                    await _appdbcontext.SaveChangesAsync();
                    return Ok(new
                    {
                        status = 200,
                        message = "Remove cart success"
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "Items does not exist", "403"));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }
        }
    }
}
