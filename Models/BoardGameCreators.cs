namespace webResfulAPIs.Models
{
    public class BoardGameCreators
    {
        public Guid Creator_Id { get; set; }
        public Creators Creator { get; set; } = null;
        public Guid BoardGame_Id { get; set; }
        public BoardGames BoardGame { get; set; } = null;
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
