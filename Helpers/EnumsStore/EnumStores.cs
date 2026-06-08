namespace webResfulAPIs.Helpers.EnumsStore
{
    public static class EnumStores
    {
        public enum UserStatus { Active, Inactive, Pending, Banned }
        public enum UserGender { Male, Female, Others }
        //creator
        public enum CreatorTypes { Author, Artist, Designer ,Publisher }
        public enum CreatorStatus { Active, Inactive, Banned }
        //boardgame
        public enum BoardGameStatus { Active, Inactive, OutStock }
        //category 
        public enum CategoryStatus { Active, Inactive, Banned }
        //bg_images

        //orders
        public enum OrderStatus { Pending, Confirmed, Cancelled, Refunded, Shipping, Delivered }
    }
}
