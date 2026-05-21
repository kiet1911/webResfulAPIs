using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webResfulAPIs.Helpers.CookiesOptions;
using webResfulAPIs.Helpers.Tokens;
using webResfulAPIs.Models;
using webResfulAPIs.Models.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BC = BCrypt.Net.BCrypt;

namespace webResfulAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {

        private AppDbContext appDbContext;
        private IConfiguration configuration;
        public AuthenticationController(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshAccesToken()
        {
            //get refresh token api cookies
            try
            {
                string? RefreshToken = null;
                HttpContext.Request.Cookies.TryGetValue("RefreshToken", out RefreshToken);
                if (string.IsNullOrEmpty(RefreshToken)) return Unauthorized("null refresh token");

                //get last refresh if not expired
                var refresh = await appDbContext.RefreshTokens.Include(t => t.User).OrderByDescending(t=>t.Expired_date).LastOrDefaultAsync(t => t.Token == RefreshToken);

                if (refresh == null || refresh.Is_revoked) return Unauthorized();

                if (refresh.Expired_date < DateTime.UtcNow)
                {
                    try
                    {
                        refresh.Is_revoked = true;

                        appDbContext.RefreshTokens.Update(refresh);
                        await appDbContext.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        return Unauthorized();
                    }

                    return Unauthorized();
                }

                //begin refresh 

                if (refresh.User == null) return Unauthorized();
                var userProfile = await appDbContext.Profiles.FirstOrDefaultAsync(t => t.User_Id == refresh.User.Id);

                if (userProfile == null) return Unauthorized();

                string Access = GenerateTokens.GenerateAccessTokensWithUser(configuration, new UserRegisterDTO { UserName = userProfile.Full_Name }, refresh.User.Public_id);
                HttpContext.Response.Cookies.Append("AccessToken", Access, CookieOptionsStore.optionForAccess);
                return Ok(new
                {
                    status = 200,
                    message = "Refresh success"
                });

            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            Users? user = await appDbContext.Users.FirstOrDefaultAsync(p => p.Email == userLoginDTO.Email);
            string fullName = "";
            //check exist email
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await appDbContext.Users.AnyAsync(p => p.Email == userLoginDTO.Email))
            {
                return BadRequest(new
                {
                    status = 400,
                    error = new
                    {
                        Email = "The Email does not exists"
                    }
                });
            }
            //check validation 
            try
            {
                bool passwordHash = BC.Verify(userLoginDTO.Password, user?.Password ?? "null");
                if (!passwordHash)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        error = new
                        {
                            Password = "Wrong Password!"
                        }
                    });
                }

                //check if refresh still invoke -> supply old else gave new refresh
                Profiles profile = await appDbContext.Profiles.FirstOrDefaultAsync(p => p.User_Id == user.Id);
                var refresh = await appDbContext.RefreshTokens.FirstOrDefaultAsync(p => p.User_id == user.Id && p.Is_revoked == false && p.Expired_date > DateTime.UtcNow);
                if (refresh != null)
                {
                    string Access = GenerateTokens.GenerateAccessTokensWithUser(configuration, new UserRegisterDTO { UserName = profile.Full_Name }, user.Public_id);
                    HttpContext.Response.Cookies.Append("AccessToken", Access, CookieOptionsStore.optionForAccess);
                    HttpContext.Response.Cookies.Append("RefreshToken", refresh.Token, CookieOptionsStore.optionForRefresh);
                }
                else
                {
                    string Refresh = GenerateTokens.GenerateRefreshTokens();
                    string Access = GenerateTokens.GenerateAccessTokensWithUser(configuration, new UserRegisterDTO { UserName = profile.Full_Name }, user.Public_id);
                    await appDbContext.RefreshTokens.AddAsync(new RefreshTokens
                    {
                        User_id = user.Id,
                        Token = Refresh,
                        Expired_date = DateTime.Now.AddDays(7),
                        Created_at = DateTime.Now,
                        Created_by_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "empty"
                    });

                    await appDbContext.SaveChangesAsync();

                    HttpContext.Response.Cookies.Append("AccessToken", Access, CookieOptionsStore.optionForAccess);
                    HttpContext.Response.Cookies.Append("RefreshToken", Refresh, CookieOptionsStore.optionForRefresh);
                }
                fullName = profile.Full_Name;
                // supply access & refresh
                //Profiles profile = await appDbContext.Profiles.FirstOrDefaultAsync(p => p.User_Id == user.Id);
                //string Refresh = GenerateTokens.GenerateRefreshTokens();
                //string Access = GenerateTokens.GenerateAccessTokensWithUser(configuration, new UserRegisterDTO { UserName = profile.Full_Name  }, user.Public_id);
                //HttpContext.Response.Cookies.Append("AccessToken", Access, CookieOptionsStore.optionForAccess);
                //HttpContext.Response.Cookies.Append("RefreshToken", Refresh, CookieOptionsStore.optionForRefresh);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });

            }
            //check and update refresh token
            return Ok(new
            {
                publicid = user.Public_id,
                fullname = fullName
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDTO)
        {
            if (System.String.IsNullOrWhiteSpace(userRegisterDTO.UserName) || System.String.IsNullOrWhiteSpace(userRegisterDTO.Email) || System.String.IsNullOrWhiteSpace(userRegisterDTO.Password))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "The request must not have null or empty agruments"
                });
            }
            //check email 
            if (await appDbContext.Users.AnyAsync((p => p.Email == userRegisterDTO.Email.Trim().ToLower())))
            {
                return Conflict(new
                {
                    status = 409,
                    message = "The email already exists"
                });
            }
            //hash password
            string passwordHash = BC.HashPassword(userRegisterDTO.Password);

            //create profile & user 
            Users user = new Users
            {
                Email = userRegisterDTO.Email.Trim().ToLower(),
                Password = passwordHash,
            };
            Profiles profile = new Profiles
            {
                Full_Name = "Anonymous",
                Display_Name = userRegisterDTO.UserName.Trim(),
                User = user
            };

            try
            {
                await appDbContext.Profiles.AddAsync(profile);
                await appDbContext.SaveChangesAsync();
                //setup cookie 
                string Refresh = GenerateTokens.GenerateRefreshTokens();
                string Access = GenerateTokens.GenerateAccessTokensWithUser(configuration, userRegisterDTO, user.Public_id);

                //try add refresh 
                await appDbContext.RefreshTokens.AddAsync(new RefreshTokens
                {
                    User_id = user.Id,
                    Token = Refresh,
                    Expired_date = DateTime.Now.AddDays(7),
                    Created_at = DateTime.Now,
                    Created_by_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "empty"
                });

                await appDbContext.SaveChangesAsync();

                HttpContext.Response.Cookies.Append("AccessToken", Access, CookieOptionsStore.optionForAccess);
                HttpContext.Response.Cookies.Append("RefreshToken", Refresh, CookieOptionsStore.optionForRefresh);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message.ToString()
                });
            }

            //generate access & refresh 
            return Ok(new
            {
                status = 200,
                message = "Create account Success!"
            });
        }
    }
}
