using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public EnumStores.CategoryStatus Status { get; set; } = EnumStores.CategoryStatus.Active;
    }
}
