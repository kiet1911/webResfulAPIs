using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class Profiles
    {
        public int Id { get; set; }
        public string Full_Name { get; set; } = "Profile";
        public string Display_Name { get; set; } = "Name";
        public string? Image_Url { get; set; }
        public DateTime Birth { get; set; } = new DateTime(1999, 1, 1);
        public EnumStores.UserGender? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public int User_Id { get; set; }
        public virtual Users? User { get; set; }
    }
}
