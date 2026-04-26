using Microsoft.AspNetCore.Http.HttpResults;

namespace webResfulAPIs.Models
{
    public class RefreshTokens
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public virtual Users? User { get; set; }
        public string Token { get; set; }
        public DateTime Expired_date { get; set; }
        public DateTime Created_at { get; set; }
        public bool Is_revoked { get; set; } = false;
        public string Created_by_ip { get; set; }
    }
}
