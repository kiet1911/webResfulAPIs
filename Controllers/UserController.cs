using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.ObjectHelper;
using webResfulAPIs.Models;
using webResfulAPIs.Models.DTO;
using BC = BCrypt.Net.BCrypt;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private IConfiguration configuration;
        private AppDbContext appDbContext;
        public UserController(IConfiguration _configuration, AppDbContext _appDbContext)
        {
            this.configuration = _configuration;
            this.appDbContext = _appDbContext;
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePassword([FromBody] ProfileDTO.UserPasswordDTO userPasswordDTO)
        {
            //validate
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "400"));
            }
            //compare with id in access token 
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");

            if (publicIdCookies != null && userPasswordDTO.PublicId.ToString() != publicIdCookies.Value)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User information (id) not match with request", "403"));
            }
            try
            {
                var UserInfo = await appDbContext.Users.Where(t => t.Public_id == userPasswordDTO.PublicId.ToString()).FirstOrDefaultAsync();
                if(UserInfo != null)
                {

                    
                    if((DateTime.UtcNow.AddHours(7) - UserInfo.Updated_at).Days <= 15)
                    {
                        return NotFound(new ResponseConfigure().CustomResponse("error", "Your current password must be at least 15 days old before you can update it.", "403"));
                    }

                    if(BC.Verify(userPasswordDTO.OldPassword, UserInfo.Password)){
                        UserInfo.Password = BC.HashPassword(userPasswordDTO.NewPassword);
                        UserInfo.Updated_at = DateTime.UtcNow.AddHours(7);
                        appDbContext.Update(UserInfo);
                        await appDbContext.SaveChangesAsync();

                        return Ok(new
                        {
                            status = 200,
                            message = "Update password success!"
                        });
                    }
                    return NotFound(new ResponseConfigure().CustomResponse("error", "User old password not match!", "403"));
                }
                return NotFound(new ResponseConfigure().CustomResponse("error", "User Profile not found!", "403"));

            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Serious system error!",
                    errorDetails = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserInfoById([FromRoute] Guid id)
        {
            //validate
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "400"));
            }
            //compare with id in access token 
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");

            if (publicIdCookies != null && id.ToString() != publicIdCookies.Value)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User information (id) not match with request", "403"));
            }
            //if not exist in database 
            try
            {
                var UserInfo = await appDbContext.Profiles.Where(t => t.User.Public_id == id.ToString()).Include(t => t.User).Select(t => new
                {
                    t.User.Phone,
                    t.Full_Name,
                    t.Birth,
                    t.Gender,
                    t.Address
                }).FirstOrDefaultAsync();
                if (UserInfo != null)
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "",
                        userInfo = UserInfo
                    });
                }
                return NotFound(new ResponseConfigure().CustomResponse("error", "User Profile not found!", "403"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Serious system error!",
                    errorDetails = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UserInfo([FromBody] ProfileDTO.UserProfileDTO userProfileDTO)
        {
            //validate
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseConfigure().CustomResponse("error", "information not fulfill", "400"));
            }
            //compare with id in access token 
            var publicIdCookies = User.Claims.FirstOrDefault(t => t.Type == "PublicId");

            if (publicIdCookies != null && userProfileDTO.PublicId.ToString() != publicIdCookies.Value)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseConfigure().CustomResponse("error", "User information (id) not match with request", "403"));
            }
            var transaction = await appDbContext.Database.BeginTransactionAsync();
            try
            {

                //user profile find 
                var userInfoUpdate = await appDbContext.Profiles.Where(t => t.User.Public_id == userProfileDTO.PublicId.ToString()).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Full_Name, userProfileDTO.FullName).SetProperty(b => b.Address, userProfileDTO.Adrress).SetProperty(b => b.Gender, userProfileDTO.Gender).SetProperty(b => b.Birth, userProfileDTO.Birth));

                var userPhoneUpdate = await appDbContext.Users.Where(t => t.Public_id == userProfileDTO.PublicId.ToString()).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Phone, userProfileDTO.Phone));

                await appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Update profile success!",
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    success = false,
                    message = "Serious system error!",
                    errorDetails = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }

        }
    }
}
