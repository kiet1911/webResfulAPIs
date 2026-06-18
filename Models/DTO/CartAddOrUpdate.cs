namespace webResfulAPIs.Models.DTO
{
    public class CartAddOrUpdate
    {
        public string publicId { get; set; }
        public Guid? boardgameId { get; set; }
        public bool isIncrease { get; set; } = true;
    }
}
