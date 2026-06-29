using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models.DTO
{
    public static class ProfileDTO
    {
        public class UserProfileDTO
        {
            public Guid PublicId { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string Adrress { get; set; }
            public EnumStores.UserGender Gender { get; set; }
            public DateTime Birth { get; set; }
        }
        public class UserPasswordDTO
        {
            public Guid PublicId { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
