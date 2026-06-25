namespace webResfulAPIs.Models.DTO
{
    public static class VnPayDTO
    {
        public class VnPayRecipientInfoDTO
        {
            public string JWT { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Note { get; set; }

        }
        public class TransactionStatus
        {
            public string PublicId { get; set; }
            public Guid OrderId { get; set; }
        }
        public class VnPayUrlResult
        {
            public string Url { get; set; }
            public DateTime DateTime { get; set; }
        }
        public class VnPayConfirmResult
        {
            public string PublicId { get; set; }
            public string txnRef { get; set; }
            public string responseCode { get; set; }
            public string transactionStatus { get; set; }
        }
    }
}
