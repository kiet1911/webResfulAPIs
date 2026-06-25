using System.Security.Cryptography;
using System.Text;
using webResfulAPIs.Models.DTO;

namespace webResfulAPIs.Services.Payment
{
    public static class PaymentService
    {
        private static readonly string _queryUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        public static VnPayDTO.VnPayUrlResult CreatePaymentUrl(Guid OrderId, decimal amount, IConfiguration configuration)
        {
            var tmnCode = configuration["VnPaySecret:TmnCode"]
          ?? throw new Exception("TmnCode not found");

            var returnUrl = configuration["VnPaySecret:ReturnUrl"]
                ?? throw new Exception("ReturnUrl not found");

            var hashSecret = configuration["VnPaySecret:HashSecret"]
                ?? throw new Exception("HashSecret not found");

            var baseUrl = configuration["VnPaySecret:BaseUrl"]
                ?? throw new Exception("BaseUrl not found");

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);

            vnpay.AddRequestData(
                "vnp_Amount",
                ((long)(amount * 100)).ToString());

            var vnpayDateCreate = DateTime.UtcNow.AddHours(7);

            vnpay.AddRequestData(
                "vnp_CreateDate",
                vnpayDateCreate.ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData(
                "vnp_ExpireDate",
                DateTime.UtcNow.AddHours(7)
                    .AddMinutes(15)
                    .ToString("yyyyMMddHHmmss"));

            vnpay.AddRequestData(
                "vnp_IpAddr",
               "127.0.0.1");

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData(
                "vnp_OrderInfo",
                $"Thanh toan don hang {OrderId}");

            vnpay.AddRequestData(
                "vnp_TxnRef",
                $"{OrderId}");

            vnpay.AddRequestData(
                "vnp_ReturnUrl",
                returnUrl);

            //return new
            //{
            //    VnPayUrl = vnpay.CreateRequestUrl(
            //    baseUrl,
            //    hashSecret),
            //    VnPayDateCreate = vnpayDateCreate
            //};
            return new VnPayDTO.VnPayUrlResult
            {
                Url = vnpay.CreateRequestUrl(
                baseUrl,
                hashSecret),
                DateTime = vnpayDateCreate
            };
        }

        public static async Task<string> CheckPaymentStatusAsync(string txnRef, string transactionDate, IConfiguration configuration)
        {
            var _tmnCode = configuration["VnPaySecret:TmnCode"]
            ?? throw new Exception("TmnCode not found");
            var _hashSecret = configuration["VnPaySecret:HashSecret"]
            ?? throw new Exception("HashSecret not found");

            string requestId = Guid.NewGuid().ToString("N");
            string version = "2.1.0";
            string command = "querydr";
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string ipAddress = "127.0.0.1";
            string orderInfo = $"Kiem tra giao dich {txnRef}";

            string rawData =
            $"{requestId}|{version}|{command}|{_tmnCode}|{txnRef}|{transactionDate}|{createDate}|{ipAddress}|{orderInfo}";
            Console.WriteLine(rawData);
            string secureHash = HmacSha512(_hashSecret, rawData);

            var requestBody = new
            {
                vnp_RequestId = requestId,
                vnp_Version = version,
                vnp_Command = command,
                vnp_TmnCode = _tmnCode,
                vnp_TxnRef = txnRef,
                vnp_OrderInfo = orderInfo,
                vnp_TransactionDate = transactionDate,
                vnp_CreateDate = createDate,
                vnp_IpAddr = ipAddress,
                vnp_SecureHash = secureHash
            };
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0");
            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(
               json,
               Encoding.UTF8,
               "application/json");

            var response = await client.PostAsync("https://sandbox.vnpayment.vn/merchant_webapi/api/transaction", content);
            var responseText = await response.Content.ReadAsStringAsync();
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return responseText;
        }
        private static string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                foreach (byte b in hashBytes)
                {
                    hash.Append(b.ToString("x2"));
                }
            }
            return hash.ToString();
        }

    }
}
