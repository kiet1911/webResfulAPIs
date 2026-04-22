using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Public_id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? Email_verify { get; set; }
        public EnumStores.UserStatus Status { get; set; } = EnumStores.UserStatus.Active;
        public string Role { get; set; } = "User";
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public DateTime? Deleted_at { get;set; }
    }
}
