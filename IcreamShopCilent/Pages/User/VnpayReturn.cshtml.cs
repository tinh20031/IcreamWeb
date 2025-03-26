using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IcreamShopClient.Pages.User
{
    public class VnpayReturnModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public VnpayReturnModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public VnPaymentResponseModel VnpayResponse { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var query = Request.Query;
            VnpayResponse = new VnPaymentResponseModel
            {
                Success = query["vnp_ResponseCode"] == "00",
                OrderId = query["vnp_TxnRef"],
                TransactionId = query["vnp_TransactionNo"],
                VnPayResponseCode = query["vnp_ResponseCode"],
                OrderDescription = query["vnp_OrderInfo"]
            };

            if (VnpayResponse.Success)
            {
                // Gọi API để cập nhật trạng thái đơn hàng
                var updateOrderUrl = "https://localhost:7283/api/CartApi/update-order-status";
                var content = new StringContent(
                    JsonSerializer.Serialize(new { OrderId = VnpayResponse.OrderId, Status = "Paid" }),
                    Encoding.UTF8,
                    "application/json"
                );
                var response = await _httpClient.PostAsync(updateOrderUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["PaymentMessage"] = "Thanh toán thành công nhưng không thể cập nhật trạng thái đơn hàng.";
                }
                else
                {
                    TempData["PaymentMessage"] = "Thanh toán thành công!";
                }
            }
            else
            {
                TempData["PaymentMessage"] = $"Thanh toán thất bại. Mã lỗi: {VnpayResponse.VnPayResponseCode}";
            }

            return Page();
        }
    }
}