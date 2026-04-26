namespace webResfulAPIs.Models
{
    public class BoardGameImages
    {
        public Guid Id { get; set; }
        public Guid Bg_id { get; set; }
        public virtual BoardGames BoardGame { get; set; } = new BoardGames();
        public string Img_Url { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
        public bool Is_Thumbnail { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
