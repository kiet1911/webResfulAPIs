using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class Creators
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public EnumStores.CreatorTypes Type { get; set; } = EnumStores.CreatorTypes.Author;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public EnumStores.CreatorStatus Status { get; set; } = EnumStores.CreatorStatus.Active;
    }
}
