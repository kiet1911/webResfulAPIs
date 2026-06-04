namespace webResfulAPIs.Models.DTO
{
    public class BoardGamesFilter
    {
        public decimal Price { get; set; }
        public int Playtime { get; set; }
        public decimal Rating { get; set; }
        public decimal Complexity { get; set; }
        public int Age { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class BoardGamesSearch
    {
        public string Search { get; set; } = string.Empty;
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
