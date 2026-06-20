using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using webResfulAPIs.Models;

namespace webResfulAPIs.Services.Payment
{
    public static class PaymentService
    {
        public static string CreatePaymentUrl(int OrderId, decimal amount, IConfiguration configuration)
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

            vnpay.AddRequestData(
                "vnp_CreateDate",
                DateTime.UtcNow.AddHours(7)
                    .ToString("yyyyMMddHHmmss"));

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
                $"{OrderId}_{DateTime.Now.Ticks}");

            vnpay.AddRequestData(
                "vnp_ReturnUrl",
                returnUrl);

            return vnpay.CreateRequestUrl(
                baseUrl,
                hashSecret);
        }

    }
}
