namespace webResfulAPIs.Models
{
    public class OrderItems
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public virtual Orders? Order { get; set; }
        public Guid BoardgameId { get; set; }
        public virtual BoardGames? BoardGame { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime Created_at { get; set; }
    }
}
