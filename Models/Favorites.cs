namespace webResfulAPIs.Models
{
    public class Favorites
    {
        public Guid BoardGame_Id { get; set; }
        public BoardGames BoardGame { get; set; }
        public int User_Id { get; set; }
        public Users User { get; set; }
        public DateTime Created_at { get; set; }
    }
}
