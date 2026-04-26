namespace webResfulAPIs.Helpers.CookiesOptions
{
    public static class CookieOptionsStore
    {
        public static CookieOptions optionForRefresh = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7),
            MaxAge = TimeSpan.FromDays(7),
            SameSite = SameSiteMode.None,
            Path = "/"
        };
        public static CookieOptions optionForAccess = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7),
            MaxAge = TimeSpan.FromDays(7),
            SameSite = SameSiteMode.None,
            Path = "/"
        };
    }
}
