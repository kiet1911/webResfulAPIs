using System.ComponentModel.DataAnnotations.Schema;
using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class BoardGames
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Base_Price {  get; set; }
        public int Stock_Quantity { get; set; }
        public int Sold_Quantity { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public EnumStores.BoardGameStatus Status { get; set; } = EnumStores.BoardGameStatus.Active;
        public decimal Weight { get; set; }
        public decimal Size_X { get; set; }
        public decimal Size_Y { get; set; }
        public decimal Size_Z { get; set; }
        public int Min_Player { get; set; }
        public int Max_Player { get; set; }
        public int Min_Time { get; set; }
        public int Max_Time { get; set; }
        public int Prefer_Player { get; set; }
        public decimal Complexity { get; set; }
        public decimal Rating { get; set; }
        public int Age_Requirement { get; set; }
        public virtual ICollection<BoardGameImages> BoardGameImages { get; set; } = new List<BoardGameImages>();
        public virtual ICollection<BoardGameCategory> BoardGameCategories { get; set; } = new List<BoardGameCategory>();
        public virtual ICollection<BoardGameCreators> BoardGameCreators { get; set; } = new List<BoardGameCreators>();
        public virtual ICollection<Favorites> Favorites { get; set; } = new List<Favorites>();
        public void handleUpdatedTime()
        {
            this.Updated_at = DateTime.UtcNow;
        }
    }
}
