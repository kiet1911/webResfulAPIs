using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace webResfulAPIs.Models
{
    public class Carts
    {
        public long Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual Users? User { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
    }
}
