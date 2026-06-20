using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace webResfulAPIs.Helpers.Tokens
{
    public static class DecryptToken
    {
        public static async Task<object> DecryptJWTsnapShotToken(string JWT, IConfiguration configuration)
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
                var result = await handler.ValidateTokenAsync(JWT, tokenDescriptor);
                if (!result.IsValid)
                {
                    if(result.Exception is SecurityTokenExpiredException)
                    {
                        throw new Exception("Token expired");
                    }
                    throw new Exception(result.Exception?.Message ?? "Invalid token");
                }
                if (result?.Claims != null &&
                 result.Claims.TryGetValue("cartSnapshot", out var value) &&
                 !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return value;
                }
                return null;
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
