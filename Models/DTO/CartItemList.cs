namespace webResfulAPIs.Models.DTO
{
    public static class CartItemList
    {
        //cart item snapshot 
        public class CartItemSnapshot
        {
            public Guid CartId { get; set; }
            public int Quantity { get; set; }
            public decimal? UnitPrice { get; set; }
        }
    }
}
