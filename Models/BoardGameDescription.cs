namespace webResfulAPIs.Models
{
    public class BoardGameDescription
    {
        public int Id { get; set; }
        public Guid BoardGame_Id { get; set; }
        public BoardGames BoardGames { get; set; } = null;
        public string Short_Description { get; set; } = string.Empty;
        public string Full_Description { get; set; } = string.Empty;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
