using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.ObjectHelper;
using webResfulAPIs.Helpers.Tokens;
using webResfulAPIs.Models;
using webResfulAPIs.Models.DTO;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private IConfiguration configuration;
        private AppDbContext appDbContext;
        public OrderController(IConfiguration configuration, AppDbContext appDbContext)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SnapShotOrderItem([FromBody] List<CartItemList.CartItemSnapshot> cartItemSnapshots)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "401"));
            }
            //user public id 
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            var UserCart = appDbContext.Carts.Where(t => t.User != null && publicIdCookies != null && t.User.Public_id == publicIdCookies.Value.ToString()).FirstOrDefault();

            if (publicIdCookies == null || UserCart == null) return StatusCode(StatusCodes.Status403Forbidden,
            new ResponseConfigure().CustomResponse("error", "User information or cart does not exist", "403")
            );
            try
            {
                var ids = cartItemSnapshots.Select(t => t.CartId).ToList();

                var cartUser = await appDbContext.CartItems.Where(t => t.CartId == UserCart.Id && ids.Contains(t.BoardgameId)).Include(t => t.BoardGames).ToListAsync();

                if (cartUser.Count() != cartItemSnapshots.Count())
                {
                    return BadRequest(new ResponseConfigure().CustomResponse("error", "Cart Item was missing or does not exist", "401"));
                }

                foreach (var i in cartUser)
                {
                    if (i != null && i.BoardGames != null)
                    {
                        i.UnitPrice = i.BoardGames.Base_Price;
                    }
                    //else
                    //{
                    //    break;
                    //    return BadRequest(new ResponseConfigure().CustomResponse("error", "Cart Item was missing or does not exist", "401"));
                    //}
                }

                var cartSnapshot = cartUser.Select(t => new CartItemList.CartItemSnapshot { CartId = t.BoardgameId, Quantity = t.Quantity , UnitPrice = t.UnitPrice }).ToList();

                var tokenSnapshot = GenerateTokens.GenerateSnapshotOrder(configuration, cartSnapshot);

                //update 

                //appDbContext.CartItems.UpdateRange(cartUser);
                //await appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "",
                    cartItems = cartUser.Select(t=>new {
                        CartId = t.BoardgameId,
                        Name = t.BoardGames.Name,
                        Quantity = t.Quantity,
                        t.UnitPrice
                    
                    }).ToList(),
                    token = tokenSnapshot
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }
        }
       
    }
}
