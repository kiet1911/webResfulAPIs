
namespace webResfulAPIs.Models
{
    public class CartItems
    {
        public long CartId { get; set; }
        public virtual Carts? Cart { get; set; }
        public Guid BoardgameId { get; set; }
        public virtual BoardGames? BoardGames { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        //public decimal? Discount { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
