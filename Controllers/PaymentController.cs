using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.EnumsStore;
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
        [HttpPut]
        public async Task<IActionResult> ConfirmPayment([FromBody] VnPayDTO.VnPayConfirmResult vnPayConfirmResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "400"));
            }
            var transaction = await appDbContext.Database.BeginTransactionAsync();
            try
            {
                //user public id 
                var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");

                if (publicIdCookies != null && vnPayConfirmResult.PublicId != publicIdCookies.Value)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User information (id) not match with request", "403"));
                }

                var UserCart = appDbContext.Carts.Where(t => t.User != null && publicIdCookies != null && t.User.Public_id == publicIdCookies.Value.ToString()).FirstOrDefault();

                if (publicIdCookies == null || UserCart == null) return StatusCode(StatusCodes.Status403Forbidden,
                new ResponseConfigure().CustomResponse("error", "User information or cart does not exist", "403")
                );

                //get orders
               
                var Order = await appDbContext.Orders.Where(t => t.CartId == UserCart.Id && t.Id.ToString() == vnPayConfirmResult.txnRef.ToUpper()).FirstOrDefaultAsync();
                if (Order == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User Order does not exist", "403")
                );
                }

                //update if match
                if(vnPayConfirmResult.responseCode == "00" && vnPayConfirmResult.transactionStatus == "00")
                {
                    if(Order.Status == EnumStores.OrderStatus.Pending)
                    {
                        Order.Status = EnumStores.OrderStatus.Confirmed;
                        Order.Paid_at = Order.Created_at;

                        var listOrderItems = await appDbContext.OrderItems.Where(t => t.OrderId == Order.Id).Include(x => x.BoardGame).ToListAsync();
                        var listBoardGame = new List<BoardGames>();

                        try
                        {
                            if (listOrderItems != null)
                            {
                                foreach (var item in listOrderItems)
                                {
                                    if (item != null && item.BoardGame != null)
                                    {
                                        var newGame = item.BoardGame;
                                        newGame.Reservation_Quantity = newGame.Reservation_Quantity - item.Quantity;
                                        newGame.Stock_Quantity = newGame.Stock_Quantity - 1;
                                        newGame.Sold_Quantity = newGame.Sold_Quantity + 1;
                                        listBoardGame.Add(newGame);
                                    }
                                }
                            }
                            //update 
                            appDbContext.Orders.Update(Order);
                            appDbContext.BoardGames.UpdateRange(listBoardGame);
                            await appDbContext.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return Ok(new
                            {
                                status = 200,
                                message = "Paid SuccessFul!",
                            });
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest(new ResponseConfigure().CustomResponse("error", "Save fail", "400"));
                        }

                    }
                }

                //error 
                return BadRequest(new ResponseConfigure().CustomResponse("error", "Something went wrong!", "403"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }


        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vnpay([FromBody] VnPayDTO.VnPayRecipientInfoDTO vnPayRecipientInfoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "401"));
            }
            var publicId = User.Claims.FirstOrDefault(t => t.Type == "PublicId");
            if (publicId == null) { return NotFound(new ResponseConfigure().CustomResponse("error", "User info not found", "404")); }
            try
            {
                var data = DecryptToken.DecryptJWTsnapShotToken(vnPayRecipientInfoDTO.JWT, configuration);
                if (data != null && data.Result != null)
                {
                    var snapshotData = System.Text.Json.JsonSerializer.Deserialize<List<CartItemList.CartItemSnapshot>>(data.Result.ToString());
                    var selectedCartItemIds = snapshotData?.Select(t => new { t.CartId, t.Quantity, t.UnitPrice }).ToList();
                    //1, check if snapshot cart really exist
                    var cartUser = await appDbContext.Carts.Where(t => t.User != null && t.User.Public_id == publicId.Value.ToString()).Include(t => t.CartItems).FirstOrDefaultAsync();
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
                        if (ids == null) return NotFound(new ResponseConfigure().CustomResponse("error", "Snapshot data not found", "404"));
                        var acutualUserItemCheckout = await appDbContext.CartItems.Where(t => t.CartId == cartUser.Id && ids.Contains(t.BoardgameId)).ToListAsync();
                        var actualCartItemIds = acutualUserItemCheckout.Select(t => t.BoardgameId).ToHashSet();
                        var isMatched = actualCartItemIds.SetEquals(ids);
                        if (!isMatched)
                        {
                            return BadRequest(new ResponseConfigure().CustomResponse("error", "The shopping basket list does not match the system data", "400"));
                        }

                        //2,1 reservation check round 
                        //var dictionarySnapshotCartItems = selectedCartItemIds.ToDictionary(t => t.CartId);
                        //var result = await appDbContext.BoardGames.Where(t => ids.Contains(t.Id)).ToListAsync();
                        foreach (var items in snapshotData)
                        {
                            int rowEffect = await appDbContext.BoardGames.Where(t => t.Id == items.CartId && t.Stock_Quantity >= t.Reservation_Quantity + items.Quantity).ExecuteUpdateAsync(
                                setter => setter.SetProperty(bg => bg.Reservation_Quantity, bg => bg.Reservation_Quantity + items.Quantity));
                            if (rowEffect == 0)
                            {
                                throw new InvalidOperationException($"BoardGame {items.CartId} does not exist or has insufficient stock.");
                            }
                        }
                        //2,2 create order + orderItems 
                        var orderUser = new Orders
                        {
                            CartId = cartUser.Id,
                            TotalPrice = acutualUserItemCheckout.Sum(t => t.Quantity * t.UnitPrice),
                            NameRecipient = vnPayRecipientInfoDTO.FullName,
                            Address = vnPayRecipientInfoDTO.Address,
                            Phone = vnPayRecipientInfoDTO.Phone,
                            note = vnPayRecipientInfoDTO.Note,
                            Status = EnumStores.OrderStatus.Pending,
                            IsSuccessDelivery = false,

                        };
                        await appDbContext.Orders.AddAsync(orderUser);
                        await appDbContext.SaveChangesAsync();

                        List<OrderItems> orderItemsUser = new List<OrderItems>();
                        foreach (var item in snapshotData)
                        {
                            OrderItems orderItems = new OrderItems
                            {
                                OrderId = orderUser.Id,
                                BoardgameId = item.CartId,
                                UnitPrice = item.UnitPrice.GetValueOrDefault(),
                                Quantity = item.Quantity,
                            };
                            orderItemsUser.Add(orderItems);
                        }

                        await appDbContext.OrderItems.AddRangeAsync(orderItemsUser);
                        await appDbContext.SaveChangesAsync();

                        //2,3 remove cartItems
                        appDbContext.CartItems.RemoveRange(acutualUserItemCheckout);
                        await appDbContext.SaveChangesAsync();


                        //request vnpay url

                        var VnPayUrl = PaymentService.CreatePaymentUrl(orderUser.Id, orderUser.TotalPrice, configuration);


                        orderUser.UrlVnPay = VnPayUrl.Url;
                        orderUser.MerchantRefNo = orderUser.Id.ToString();
                        orderUser.Created_at = VnPayUrl.DateTime;

                        appDbContext.Orders.Update(orderUser);

                        await appDbContext.SaveChangesAsync();
                        //return

                        await transaction.CommitAsync();

                        return Ok(new { Data = snapshotData, VnPayUrl = VnPayUrl.Url });


                    }
                    catch (InvalidOperationException ex)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "400"));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "400"));
                    }

                }


                return BadRequest(new ResponseConfigure().CustomResponse("error", "No item was found", "401"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }

        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> TransactionStatus([FromBody] VnPayDTO.TransactionStatus transactionStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "400"));
            }
            //user public id 
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");

            if (publicIdCookies != null && transactionStatus.PublicId != publicIdCookies.Value)
            {
                new ResponseConfigure().CustomResponse("error", "User information (id) not match with request", "403");
            }

            var UserCart = appDbContext.Carts.Where(t => t.User != null && publicIdCookies != null && t.User.Public_id == publicIdCookies.Value.ToString()).FirstOrDefault();

            if (publicIdCookies == null || UserCart == null) return StatusCode(StatusCodes.Status403Forbidden,
            new ResponseConfigure().CustomResponse("error", "User information or cart does not exist", "403")
            );
            try
            {
                //find order
                var Order = await appDbContext.Orders.Where(t => t.Id == transactionStatus.OrderId && t.CartId == UserCart.Id).FirstOrDefaultAsync();
                if (Order == null)
                {
                    new ResponseConfigure().CustomResponse("error", "Order not found", "403");
                }
                //process check 
                var dateTimeString = Order.Created_at.ToString("yyyyMMddHHmmss");
                var data = await PaymentService.CheckPaymentStatusAsync(Order.MerchantRefNo, dateTimeString, configuration);

                //check if vnp_ResponseCode = 00

                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(data);
                if (dict != null && Order.Status == EnumStores.OrderStatus.Pending)
                {
                    dict.TryGetValue("vnp_TransactionStatus", out object TransactStatus);
                    dict.TryGetValue("vnp_ResponseCode", out object ResponseCode);
                    if (ResponseCode != null && ResponseCode.ToString() == "00")
                    {
                        if (TransactStatus != null && TransactStatus.ToString() == "00")
                        {
                            //update if order is peding 
                            if (Order.Status == EnumStores.OrderStatus.Pending)
                            {
                                using var transaction = await appDbContext.Database.BeginTransactionAsync();
                                Order.Status = EnumStores.OrderStatus.Confirmed;
                                Order.Paid_at = Order.Created_at;
                                //update boardgame reservation and sold
                                var listOrderItems = await appDbContext.OrderItems.Where(t => t.OrderId == Order.Id).Include(x => x.BoardGame).ToListAsync();
                                var listBoardGame = new List<BoardGames>();

                                try
                                {
                                    if (listOrderItems != null)
                                    {
                                        foreach (var item in listOrderItems)
                                        {
                                            if (item != null && item.BoardGame != null)
                                            {
                                                var newGame = item.BoardGame;
                                                newGame.Reservation_Quantity = newGame.Reservation_Quantity - item.Quantity;
                                                newGame.Stock_Quantity = newGame.Stock_Quantity - 1;
                                                newGame.Sold_Quantity = newGame.Sold_Quantity + 1;
                                                listBoardGame.Add(newGame);
                                            }
                                        }
                                    }
                                    //update 
                                    appDbContext.Orders.Update(Order);
                                    appDbContext.BoardGames.UpdateRange(listBoardGame);
                                    await appDbContext.SaveChangesAsync();

                                    await transaction.CommitAsync();
                                    return Ok(new
                                    {
                                        status = 200,
                                        message = "Update status success",
                                    });
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    return BadRequest(new ResponseConfigure().CustomResponse("error", "Save fail", "400"));
                                }

                            }

                            return Ok(new
                            {
                                status = 201,
                                message = "Nothing!"
                            });
                            //
                        }
                        else if (TransactStatus != null && (TransactStatus.ToString() == "08" || TransactStatus.ToString() == "11"))
                        {
                            if (Order.Status == EnumStores.OrderStatus.Pending && DateTime.UtcNow.AddHours(7) > Order.Created_at.AddMinutes(7))
                            {
                                //reject order -> 
                                Order.Status = EnumStores.OrderStatus.Cancelled;
                                Order.Deleted_at = DateTime.UtcNow.AddHours(7);

                                //refund reservation 
                                var listOrderItems = await appDbContext.OrderItems.Where(t => t.OrderId == Order.Id).Include(x => x.BoardGame).ToListAsync();
                                var listBoardGame = new List<BoardGames>();
                                using var transaction = await appDbContext.Database.BeginTransactionAsync();
                                try
                                {
                                    if (listOrderItems != null)
                                    {
                                        foreach (var item in listOrderItems)
                                        {
                                            if (item != null && item.BoardGame != null)
                                            {
                                                var newGame = item.BoardGame;
                                                newGame.Reservation_Quantity = newGame.Reservation_Quantity - item.Quantity;
                                                listBoardGame.Add(newGame);
                                            }
                                        }
                                    }
                                    //update 
                                    appDbContext.Orders.Update(Order);
                                    appDbContext.BoardGames.UpdateRange(listBoardGame);
                                    await appDbContext.SaveChangesAsync();

                                    await transaction.CommitAsync();

                                    return Ok(new
                                    {
                                        status = 200,
                                        message = "The order is past the payment deadline, refund reservation."
                                    });
                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync();
                                    return BadRequest(new ResponseConfigure().CustomResponse("error", "Save fail", "400"));
                                }

                            }
                            else
                            {
                                return BadRequest(new ResponseConfigure().CustomResponse("error", $"See Detail Transaction in detail order {TransactStatus.ToString()} ", "400"));
                            }

                        }
                        return BadRequest(new ResponseConfigure().CustomResponse("error", $"See Detail Transaction in detail order {TransactStatus.ToString()} ", "400"));

                    }
                    else
                    {
                        return BadRequest(new ResponseConfigure().CustomResponse("error", "Rate limit", "400"));
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "No Action"
                    });
                }


                //if(dict.TryGetValue("vnp_ResponseCode", out object value))
                //{
                //    if (value.ToString() == "00")
                //    {
                //        return Ok(dict);
                //    }
                //}

                //return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", ex.Message, "401"));
            }
        }
    }
}
