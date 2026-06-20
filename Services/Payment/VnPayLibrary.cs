using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace webResfulAPIs.Services.Payment
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData =
            new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();

            foreach (var kv in _requestData)
            {
                if (data.Length > 0)
                    data.Append("&");

                data.Append(WebUtility.UrlEncode(kv.Key));
                data.Append("=");
                data.Append(WebUtility.UrlEncode(kv.Value));
            }

            string queryString = data.ToString();

            using var hmac =
                new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret));

            var hashBytes = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(queryString));

            string secureHash =
                Convert.ToHexString(hashBytes).ToLower();

            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }
    }
}
