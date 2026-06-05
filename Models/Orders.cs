using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class Orders
    {
        public Guid Id { get; set; }
        public long CartId { get; set; }
        public virtual Carts? Cart { get; set; }
        public decimal TotalPrice { get; set; }
        public EnumStores.OrderStatus Status { get; set; }
        public Boolean IsSuccessDelivery { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Paid_at { get; set; }
        public DateTime? Deleted_at { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
