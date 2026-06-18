using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace webResfulAPIs.Helpers.Tokens
{
    public static class DecryptToken
    {
        public static async Task DecryptJWTsnapShotToken(string JWT, IConfiguration configuration)
        {
            try
            {
                var secretkey = configuration["Secret:keys"] ?? throw new Exception("secret key not found");
                var tokenDescriptor = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["Secret:audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Secret:issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey)),
                };
                var handler = new JsonWebTokenHandler();
                var ClaimPrinciples = await handler.ValidateTokenAsync(JWT, tokenDescriptor);
                if (ClaimPrinciples != null)
                {
                    ClaimPrinciples.Claims.TryGetValue("productions", out var value);
                    
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
