namespace webResfulAPIs.Helpers.EnumsStore
{
    public static class EnumStores
    {
        public enum UserStatus { Active, Inactive, Pending, Banned }
        public enum UserGender { Male, Female, Others }
        public enum CreatorTypes { Author, Artist }
        public enum CreatorStatus { Active, Inactive, Banned }
        //boardgame
        public enum BoardGameStatus { Active, Inactive, OutStock }
        //category 
        public enum CategoryStatus { Active, Inactive, Banned }
        //bg_images
    }
}
