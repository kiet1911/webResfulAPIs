

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using webResfulAPIs.Models.DTO;

namespace webResfulAPIs.Helpers.Tokens
{
    public static class GenerateTokens
    {
        public static string GenerateAccessTokens(IConfiguration configuration)
        {
            string secretKey = configuration["Secret:keys"] ?? throw new Exception("secret key not found");
            var symmetric = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes(secretKey));
            var sign = new SigningCredentials(symmetric, SecurityAlgorithms.HmacSha256);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity() { },
                IssuedAt = DateTime.UtcNow,
                Issuer = configuration["Secret:issuer"],
                Audience = configuration["Secret:audience"],
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = sign
            };
            var handler = new JsonWebTokenHandler();
            try {
                var token = handler.CreateToken(descriptor);
                return token;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            } 
        }
        public static string GenerateAccessTokensWithUser(IConfiguration configuration, UserRegisterDTO userRegisterDTO , string public_id)
        {
            string secretKey = configuration["Secret:keys"] ?? throw new Exception("secret key not found");
            var symmetric = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes(secretKey));
            var sign = new SigningCredentials(symmetric, SecurityAlgorithms.HmacSha256);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                     new Claim(ClaimTypes.Name,userRegisterDTO.UserName),
                     new Claim("PublicId",public_id),
                    ]),
                IssuedAt = DateTime.UtcNow,
                Issuer = configuration["Secret:issuer"],
                Audience = configuration["Secret:audience"],
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = sign
            };
            var handler = new JsonWebTokenHandler();
            try
            {
                var token = handler.CreateToken(descriptor);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string GenerateRefreshTokens()
        {
            var randomString = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(randomString);
            return refreshToken;
        }
    }
}
