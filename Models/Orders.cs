using webResfulAPIs.Helpers.EnumsStore;

namespace webResfulAPIs.Models
{
    public class Orders
    {
        public Guid Id { get; set; }
        public long CartId { get; set; }
        public virtual Carts? Cart { get; set; }
        public decimal TotalPrice { get; set; }
        public string NameRecipient { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? note { get; set; }
        public EnumStores.OrderStatus Status { get; set; }
        public Boolean IsSuccessDelivery { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Paid_at { get; set; }
        public DateTime? Deleted_at { get; set; }
        public string UrlVnPay { get; set; } = string.Empty;
        public string MerchantRefNo { get; set; } = string.Empty;

        
        public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
