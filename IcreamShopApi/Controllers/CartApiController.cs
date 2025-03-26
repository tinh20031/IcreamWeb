using IcreamShopApi.Data;
using IcreamShopApi.DTOs;
using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly CreamDbContext _context;
        private readonly VNPAYConfig _vnpayConfig;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartApiController> _logger;

        public CartApiController(
            CartService cartService,
            OrderService orderService,
            CreamDbContext context,
            IOptions<VNPAYConfig> vnpayConfig,
            HttpClient httpClient,
            ILogger<CartApiController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _context = context;
            _vnpayConfig = vnpayConfig.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cart>>> GetAllCarts()
        {
            var carts = await _cartService.GetAllCarts();
            return Ok(carts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCartById(int id)
        {
            var getCartId = await _cartService.GetCartById(id);
            return Ok(getCartId);
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> AddCart([FromBody] Cart cart)
        {
            var addCart = await _cartService.AddCart(cart);
            return Ok(addCart);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Cart>> EditCart(int id, [FromBody] Cart cart)
        {
            try
            {
                if (id != cart.CartId)
                {
                    return BadRequest(new { message = "CartId không khớp với ID trong URL." });
                }

                var existingCart = await _cartService.GetCartById(id);
                if (existingCart == null)
                {
                    return NotFound(new { message = $"Không tìm thấy giỏ hàng với ID {id}." });
                }

                existingCart.Quantity = cart.Quantity;
                existingCart.CreatedAt = cart.CreatedAt;

                await _cartService.EditCart(existingCart);
                return Ok(existingCart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật giỏ hàng.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cart>> DeleteCart(int id)
        {
            await _cartService.DeleteCart(id);
            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Cart>>> GetCartsByUserId(int userId)
        {
            try
            {
                var carts = await _cartService.GetCartsByUserIdAsync(userId);
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("create-order/{userId}")]
        public async Task<ActionResult<Order>> CreateOrderFromCart(int userId, [FromBody] CreateOrderRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ShippingAddress))
                {
                    return BadRequest(new { message = "Địa chỉ giao hàng không được để trống." });
                }

                var order = await _orderService.CreateOrderFromCart(userId, request.ShippingAddress);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "Không có chi tiết lỗi nội bộ.";
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo đơn hàng.", details = ex.Message, innerException = innerExceptionMessage });
            }
        }

        [HttpPost("create-payment/{userId}")]
        public async Task<IActionResult> CreatePayment(int userId, [FromBody] CreatePaymentRequest request)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new { message = "UserId không hợp lệ." });
                }

                if (request == null || string.IsNullOrWhiteSpace(request.ShippingAddress))
                {
                    return BadRequest(new { message = "Địa chỉ giao hàng không được để trống." });
                }

                // Lấy thông tin giỏ hàng
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .Join(
                        _context.IceCreams,
                        cart => cart.IceCreamId,
                        iceCream => iceCream.IceCreamId,
                        (cart, iceCream) => new CartDTO
                        {
                            CartId = cart.CartId,
                            UserId = cart.UserId,
                            IceCreamId = cart.IceCreamId,
                            Quantity = cart.Quantity,
                            CreatedAt = cart.CreatedAt,
                            IceCreamName = iceCream.Name,
                            Image = iceCream.ImageUrl,
                            Price = iceCream.Price
                        })
                    .ToListAsync();

                if (cartItems == null || cartItems.Count == 0)
                {
                    return BadRequest(new { message = "Giỏ hàng trống." });
                }

                if (cartItems.Any(item => item.Price <= 0 || item.Quantity <= 0))
                {
                    return BadRequest(new { message = "Giá hoặc số lượng không hợp lệ." });
                }

                var totalAmount = (long)(cartItems.Sum(item => item.Price * item.Quantity) * 25000);
                _logger.LogInformation("Tổng số tiền (VND): {TotalAmount}", totalAmount);
                if (totalAmount < 1000)
                {
                    return BadRequest(new { message = "Số tiền tối thiểu phải là 1,000 VND." });
                }

                // Tạo đơn hàng và lưu vào database
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = cartItems.Sum(item => item.Price * item.Quantity),
                    Status = "Chờ thanh toán", // Trạng thái ban đầu
                    ShippingAddress = request.ShippingAddress
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Thêm chi tiết đơn hàng
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        IceCreamId = item.IceCreamId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                // Xóa giỏ hàng
                var cartItemsToRemove = _context.Carts.Where(c => c.UserId == userId);
                _context.Carts.RemoveRange(cartItemsToRemove);

                await _context.SaveChangesAsync();

                // Sử dụng OrderId làm vnp_TxnRef
                var vnp_TxnRef = order.OrderId.ToString();
                _logger.LogInformation("Mã giao dịch VNPAY (vnp_TxnRef): {VnpTxnRef}", vnp_TxnRef);

                // Tạo tham số VNPAY
                var vnpayParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _vnpayConfig.TmnCode },
            { "vnp_Amount", (totalAmount * 100).ToString() },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", "127.0.0.1" }, // Cố định IPv4
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", $"ThanhToanDonHang{vnp_TxnRef}" },
            { "vnp_OrderType", "250000" },
            { "vnp_ReturnUrl", _vnpayConfig.ReturnUrl },
            { "vnp_TxnRef", vnp_TxnRef }
        };

                _logger.LogInformation("Tham số VNPAY: {VnpayParams}", string.Join(", ", vnpayParams.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                var signData = string.Join("&", vnpayParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                _logger.LogInformation("Chuỗi dữ liệu để tính checksum: {SignData}", signData);
                var vnp_SecureHash = HmacSha512(signData, _vnpayConfig.HashSecret);
                _logger.LogInformation("Chữ ký VNPAY (vnp_SecureHash): {VnpSecureHash}", vnp_SecureHash);

                vnpayParams.Add("vnp_SecureHashType", "SHA512");
                vnpayParams.Add("vnp_SecureHash", vnp_SecureHash);

                var paymentUrl = _vnpayConfig.PaymentUrl + "?" + string.Join("&", vnpayParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                _logger.LogInformation("Tạo URL thanh toán VNPAY: {PaymentUrl}", paymentUrl);

                return Ok(new { OrderUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thanh toán VNPAY cho người dùng {UserId}", userId);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo yêu cầu thanh toán.", details = ex.Message });
            }
        }

        [HttpPost("vnpay-ipn")]
        public async Task<IActionResult> VnpayIpn()
        {
            try
            {
                var vnpayData = Request.Form;
                var vnp_SecureHash = vnpayData["vnp_SecureHash"];
                var vnp_TxnRef = vnpayData["vnp_TxnRef"];
                var vnp_ResponseCode = vnpayData["vnp_ResponseCode"];
                var vnp_TransactionStatus = vnpayData["vnp_TransactionStatus"];

                // Kiểm tra chữ ký
                var hashParams = vnpayData.Keys
                    .Where(k => k != "vnp_SecureHash" && k != "vnp_SecureHashType")
                    .OrderBy(k => k)
                    .Select(k => $"{k}={Uri.EscapeDataString(vnpayData[k])}");
                var signData = string.Join("&", hashParams);
                var computedHash = HmacSha512(signData, _vnpayConfig.HashSecret);

                if (computedHash != vnp_SecureHash)
                {
                    _logger.LogWarning("Chữ ký VNPAY không hợp lệ.");
                    return BadRequest(new { RspCode = "97", Message = "Invalid checksum" });
                }

                // Tìm đơn hàng trong database
                if (!int.TryParse(vnp_TxnRef, out int orderId))
                {
                    _logger.LogWarning("Mã giao dịch VNPAY không hợp lệ: {VnpTxnRef}", vnp_TxnRef);
                    return BadRequest(new { RspCode = "01", Message = "Invalid order ID" });
                }

                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng cho vnp_TxnRef: {VnpTxnRef}", vnp_TxnRef);
                    return BadRequest(new { RspCode = "01", Message = "Order not found" });
                }

                // Cập nhật trạng thái đơn hàng
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    order.Status = "Đã thanh toán";
                    _logger.LogInformation("Cập nhật trạng thái đơn hàng thành công cho vnp_TxnRef: {VnpTxnRef}", vnp_TxnRef);
                }
                else
                {
                    order.Status = "Thanh toán thất bại";
                    _logger.LogWarning("Thanh toán VNPAY thất bại cho vnp_TxnRef: {VnpTxnRef}", vnp_TxnRef);
                }

                await _context.SaveChangesAsync();

                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý IPN VNPAY.");
                return StatusCode(500, new { RspCode = "99", Message = "Internal server error" });
            }
        }

        [HttpPost("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId.ToString() == request.OrderId);
                if (order == null)
                {
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });
                }

                order.Status = request.Status;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng cho OrderId: {OrderId}", request.OrderId);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật trạng thái đơn hàng.", details = ex.Message });
            }
        }

        private string HmacSha512(string data, string key)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }


        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://provinces.open-api.vn/api/p/");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Không thể lấy danh sách tỉnh." });
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách tỉnh.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách tỉnh.", details = ex.Message });
            }
        }

        [HttpGet("districts/{provinceCode}")]
        public async Task<IActionResult> GetDistricts(int provinceCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://provinces.open-api.vn/api/p/{provinceCode}?depth=2");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Không thể lấy danh sách quận/huyện." });
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách quận/huyện cho provinceCode {ProvinceCode}.", provinceCode);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách quận/huyện.", details = ex.Message });
            }
        }

        [HttpGet("wards/{districtCode}")]
        public async Task<IActionResult> GetWards(int districtCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://provinces.open-api.vn/api/d/{districtCode}?depth=2");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { message = "Không thể lấy danh sách phường/xã." });
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phường/xã cho districtCode {DistrictCode}.", districtCode);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách phường/xã.", details = ex.Message });
            }
        }
        [HttpPost("create-payment-app/{userId}")]
        public async Task<IActionResult> CreatePaymentforApp(int userId, [FromBody] CreatePaymentRequest request)
        {
            _logger.LogInformation("Bắt đầu xử lý CreatePaymentforApp cho userId: {UserId}", userId);
            _logger.LogInformation("Dữ liệu yêu cầu: ShippingAddress = {ShippingAddress}", request?.ShippingAddress);

            try
            {
                if (userId <= 0)
                {
                    _logger.LogWarning("UserId không hợp lệ: {UserId}", userId);
                    return BadRequest(new { message = "UserId không hợp lệ." });
                }

                if (request == null || string.IsNullOrWhiteSpace(request.ShippingAddress))
                {
                    _logger.LogWarning("Địa chỉ giao hàng không hợp lệ: {ShippingAddress}", request?.ShippingAddress);
                    return BadRequest(new { message = "Địa chỉ giao hàng không được để trống." });
                }

                // Lấy thông tin giỏ hàng
                _logger.LogInformation("Lấy giỏ hàng cho userId: {UserId}", userId);
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .Join(
                        _context.IceCreams,
                        cart => cart.IceCreamId,
                        iceCream => iceCream.IceCreamId,
                        (cart, iceCream) => new CartDTO
                        {
                            CartId = cart.CartId,
                            UserId = cart.UserId,
                            IceCreamId = cart.IceCreamId,
                            Quantity = cart.Quantity,
                            CreatedAt = cart.CreatedAt,
                            IceCreamName = iceCream.Name,
                            Image = iceCream.ImageUrl,
                            Price = iceCream.Price
                        })
                    .ToListAsync();

                _logger.LogInformation("Số lượng sản phẩm trong giỏ hàng: {Count}", cartItems?.Count ?? 0);
                if (cartItems == null || cartItems.Count == 0)
                {
                    _logger.LogWarning("Giỏ hàng trống cho userId: {UserId}", userId);
                    return BadRequest(new { message = "Giỏ hàng trống." });
                }

                if (cartItems.Any(item => item.Price <= 0 || item.Quantity <= 0))
                {
                    _logger.LogWarning("Giá hoặc số lượng không hợp lệ cho userId: {UserId}", userId);
                    return BadRequest(new { message = "Giá hoặc số lượng không hợp lệ." });
                }

                var totalAmount = (long)(cartItems.Sum(item => item.Price * item.Quantity) * 25000);
                _logger.LogInformation("Tổng số tiền (VND): {TotalAmount}", totalAmount);
                if (totalAmount < 1000)
                {
                    _logger.LogWarning("Số tiền tối thiểu không đủ: {TotalAmount}", totalAmount);
                    return BadRequest(new { message = "Số tiền tối thiểu phải là 1,000 VND." });
                }

                // Tạo đơn hàng và lưu vào database
                _logger.LogInformation("Tạo đơn hàng mới cho userId: {UserId}", userId);
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = cartItems.Sum(item => item.Price * item.Quantity),
                    Status = "Chờ thanh toán",
                    ShippingAddress = request.ShippingAddress
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã lưu đơn hàng với OrderId: {OrderId}", order.OrderId);

                // Thêm chi tiết đơn hàng
                _logger.LogInformation("Thêm chi tiết đơn hàng cho OrderId: {OrderId}", order.OrderId);
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        IceCreamId = item.IceCreamId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                // Xóa giỏ hàng
                _logger.LogInformation("Xóa giỏ hàng cho userId: {UserId}", userId);
                var cartItemsToRemove = _context.Carts.Where(c => c.UserId == userId);
                _context.Carts.RemoveRange(cartItemsToRemove);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã lưu thay đổi vào database");

                // Sử dụng OrderId làm vnp_TxnRef
                var vnp_TxnRef = order.OrderId.ToString();
                _logger.LogInformation("Mã giao dịch VNPAY (vnp_TxnRef): {VnpTxnRef}", vnp_TxnRef);

                // Tạo tham số VNPAY
                _logger.LogInformation("Tạo tham số VNPAY");
                var vnpayParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _vnpayConfig.TmnCode },
            { "vnp_Amount", (totalAmount * 100).ToString() },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", "127.0.0.1" },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", $"ThanhToanDonHang{vnp_TxnRef}" },
            { "vnp_OrderType", "250000" },
            { "vnp_ReturnUrl", _vnpayConfig.ReturnUrl },
            { "vnp_TxnRef", vnp_TxnRef }
        };

                _logger.LogInformation("Tham số VNPAY: {VnpayParams}", string.Join(", ", vnpayParams.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                var signData = string.Join("&", vnpayParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                _logger.LogInformation("Chuỗi dữ liệu để tính checksum: {SignData}", signData);
                var vnp_SecureHash = HmacSha512(signData, _vnpayConfig.HashSecret);
                _logger.LogInformation("Chữ ký VNPAY (vnp_SecureHash): {VnpSecureHash}", vnp_SecureHash);

                vnpayParams.Add("vnp_SecureHashType", "SHA512");
                vnpayParams.Add("vnp_SecureHash", vnp_SecureHash);

                var paymentUrl = _vnpayConfig.PaymentUrl + "?" + string.Join("&", vnpayParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                _logger.LogInformation("Tạo URL thanh toán VNPAY: {PaymentUrl}", paymentUrl);

                var response = new { OrderUrl = paymentUrl };
                _logger.LogInformation("Phản hồi trả về: {Response}", Newtonsoft.Json.JsonConvert.SerializeObject(response));
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thanh toán VNPAY cho người dùng {UserId}", userId);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo yêu cầu thanh toán.", details = ex.Message });
            }
        }

        [HttpGet("vnpay-return")]
        public IActionResult VnpayReturn()
        {
            _logger.LogInformation("Nhận yêu cầu từ VNPAY tại vnpay-return.");
            try
            {
                var vnpayData = Request.Query;
                _logger.LogInformation("Tham số VNPAY nhận được: {VnpayData}", vnpayData.ToString());

                var vnp_SecureHash = vnpayData["vnp_SecureHash"];
                var vnp_TxnRef = vnpayData["vnp_TxnRef"];
                var vnp_ResponseCode = vnpayData["vnp_ResponseCode"];
                var vnp_TransactionStatus = vnpayData["vnp_TransactionStatus"];

                // Kiểm tra chữ ký
                var hashParams = vnpayData.Keys
                    .Where(k => k != "vnp_SecureHash" && k != "vnp_SecureHashType")
                    .OrderBy(k => k)
                    .Select(k => $"{k}={Uri.EscapeDataString(vnpayData[k])}");
                var signData = string.Join("&", hashParams);
                _logger.LogInformation("Chuỗi dữ liệu để tính checksum: {SignData}", signData);
                var computedHash = HmacSha512(signData, _vnpayConfig.HashSecret);
                _logger.LogInformation("Chữ ký tính được: {ComputedHash}, Chữ ký nhận được: {VnpSecureHash}", computedHash, vnp_SecureHash);

                if (computedHash != vnp_SecureHash)
                {
                    _logger.LogWarning("Chữ ký VNPAY không hợp lệ.");
                    return BadRequest(new { message = "Invalid checksum" });
                }

                // Cập nhật trạng thái đơn hàng
                if (int.TryParse(vnp_TxnRef, out int orderId))
                {
                    var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order != null)
                    {
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            order.Status = "Paid";
                        }
                        else
                        {
                            order.Status = "Failed";
                        }
                        _context.SaveChanges();
                        _logger.LogInformation("Cập nhật trạng thái đơn hàng {OrderId}: {Status}", orderId, order.Status);
                    }
                }

                // Tạo Deep Link
                var deepLink = $"iceamapp://payment-callback?vnp_TxnRef={vnp_TxnRef}&vnp_ResponseCode={vnp_ResponseCode}&vnp_TransactionStatus={vnp_TransactionStatus}";
                _logger.LogInformation("Chuyển hướng về ứng dụng Android với Deep Link: {DeepLink}", deepLink);

                return Redirect(deepLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý VnpayReturn.");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xử lý kết quả thanh toán." });
            }
        }

        [HttpGet("vnpay-return-for-app")]
        public async Task<IActionResult> VnpayReturnforapp()
        {
            _logger.LogInformation("Nhận yêu cầu từ VNPAY tại vnpay-return.");
            try
            {
                var vnpayData = Request.Query;
                _logger.LogInformation("Tham số VNPAY nhận được: {VnpayData}", vnpayData.ToString());

                var vnp_SecureHash = vnpayData["vnp_SecureHash"];
                var vnp_TxnRef = vnpayData["vnp_TxnRef"];
                var vnp_ResponseCode = vnpayData["vnp_ResponseCode"];
                var vnp_TransactionStatus = vnpayData["vnp_TransactionStatus"];

                // Kiểm tra chữ ký
                var hashParams = vnpayData.Keys
                    .Where(k => k != "vnp_SecureHash" && k != "vnp_SecureHashType")
                    .OrderBy(k => k)
                    .Select(k => $"{k}={Uri.EscapeDataString(vnpayData[k])}");
                var signData = string.Join("&", hashParams);
                _logger.LogInformation("Chuỗi dữ liệu để tính checksum: {SignData}", signData);
                var computedHash = HmacSha512(signData, _vnpayConfig.HashSecret);
                _logger.LogInformation("Chữ ký tính được: {ComputedHash}, Chữ ký nhận được: {VnpSecureHash}", computedHash, vnp_SecureHash);

                if (computedHash != vnp_SecureHash)
                {
                    _logger.LogWarning("Chữ ký VNPAY không hợp lệ.");
                    return BadRequest(new { status = "error", message = "Invalid checksum" });
                }

                // Cập nhật trạng thái đơn hàng
                if (int.TryParse(vnp_TxnRef, out int orderId))
                {
                    var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order == null)
                    {
                        _logger.LogWarning("Không tìm thấy đơn hàng với OrderId: {OrderId}", orderId);
                        return NotFound(new { status = "error", message = "Order not found" });
                    }

                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        order.Status = "Paid";
                        _logger.LogInformation("Cập nhật trạng thái đơn hàng {OrderId}: Đã thanh toán", orderId);
                        await _context.SaveChangesAsync();
                        return Ok(new
                        {
                            status = "success",
                            orderId = orderId,
                            message = "Thanh toán thành công"
                        });
                    }
                    else
                    {
                        order.Status = "Failed";
                        _logger.LogInformation("Cập nhật trạng thái đơn hàng {OrderId}: Thanh toán thất bại", orderId);
                        await _context.SaveChangesAsync();
                        return Ok(new
                        {
                            status = "failure",
                            orderId = orderId,
                            message = "Thanh toán thất bại",
                            responseCode = vnp_ResponseCode
                        });
                    }
                }
                else
                {
                    _logger.LogWarning("Mã giao dịch VNPAY không hợp lệ: {VnpTxnRef}", vnp_TxnRef);
                    return BadRequest(new { status = "error", message = "Invalid order ID" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý VnpayReturn.");
                return StatusCode(500, new { status = "error", message = "Đã xảy ra lỗi khi xử lý kết quả thanh toán." });
            }
        }



    }

    public class CreateOrderRequest
    {
        public string ShippingAddress { get; set; }
    }

    public class CreatePaymentRequest
    {
        public string ShippingAddress { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
    }

    public class ZaloPayCallbackRequest
    {
        public string Data { get; set; }
        public string Mac { get; set; }
    }





}