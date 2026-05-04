namespace webResfulAPIs.Models
{
    public class BoardGameCategory
    {
        public Guid BoardGame_Id { get; set; }
        public BoardGames BoardGame { get; set; } = null;
        public int Category_Id { get;set; }
        public Categories Category { get; set; } = null;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
