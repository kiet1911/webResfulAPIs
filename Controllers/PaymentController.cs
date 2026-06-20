using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.ObjectHelper;
using webResfulAPIs.Helpers.Tokens;
using webResfulAPIs.Models;
using webResfulAPIs.Models.DTO;
using webResfulAPIs.Services.Payment;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private IConfiguration configuration;
        private AppDbContext appDbContext;
        public PaymentController(IConfiguration configuration, AppDbContext appDbContext)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vnpay([FromBody] string JWT)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "401"));
            }
            var publicId = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            if (publicId == null) { return NotFound(new ResponseConfigure().CustomResponse("error", "User info not found (crypt id)", "404")); }
            try
            {
                var data = DecryptToken.DecryptJWTsnapShotToken(JWT, configuration);
                if (data != null && data.Result != null)
                {
                    var snapshotData = System.Text.Json.JsonSerializer.Deserialize<List<CartItemList.CartItemSnapshot>>(data.Result.ToString());
                    var selectedCartItemIds = snapshotData?.Select(t => new { t.CartId, t.Quantity, t.UnitPrice }).ToList();
                    //1, check if snapshot cart really exist
                    var cartUser = await appDbContext.Carts.Where(t => t.User != null && t.User.Public_id == publicId.Value.ToString()).Include(t=>t.CartItems).FirstOrDefaultAsync();
                    if (cartUser?.UserId == null || cartUser == null)
                    {
                        return NotFound(new ResponseConfigure().CustomResponse("error", "User info not found", "404"));
                    }

                    //1,1 does not need check cart items 
                    //2, begin transaction if we can reversation the quantity of each production 
                    using var transaction = await appDbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var ids = selectedCartItemIds?.Select(t => t.CartId).ToList();
                        if(ids == null) return NotFound(new ResponseConfigure().CustomResponse("error", "Snapshot data not found", "404"));
                        var acutualUserItemCheckout = await appDbContext.CartItems.Where(t => t.CartId == cartUser.Id && ids.Contains(t.BoardgameId)).ToListAsync();
                        var actualCartItemIds = acutualUserItemCheckout.Select(t => t.BoardgameId).ToHashSet();
                        var isMatched = actualCartItemIds.SetEquals(ids);
                        if (!isMatched)
                        {
                            return BadRequest(new ResponseConfigure().CustomResponse("error", "The shopping basket list does not match the system data", "400"));
                        }

                        var VnPayUrl = PaymentService.CreatePaymentUrl(1, 10000, configuration);

                        return Ok(new { Data = snapshotData , VnPayUrl = VnPayUrl });


                    }
                    catch(Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "400"));
                    }
                    //3, update production reversation 
                    //4, try create order with recipiest info and total price , order items
                    //5, try create url vnpay 
                    //6, commit 
                    //7, return pay url 

                    
                
                
                }
               

                return BadRequest(new ResponseConfigure().CustomResponse("error", "No item was found", "401"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }

        }
    }
}
