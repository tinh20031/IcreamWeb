using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressApiController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressApiController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAddressesByUserId(int userId)
        {
            try
            {
                var addresses = await _addressService.GetAddressesByUserId(userId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{addressId}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            try
            {
                var address = await _addressService.GetAddressById(addressId);
                if (address == null)
                {
                    return NotFound(new { message = "Không tìm thấy địa chỉ." });
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAddress([FromBody] Address address)
        {
            try
            {
                // Kiểm tra quyền truy cập
                var tokenUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (tokenUserId != address.UserId)
                {
                    return Forbid("Bạn không có quyền thêm địa chỉ cho người dùng này.");
                }

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(address.Street))
                
                {
                    return BadRequest(new { message = "Các trường Street, City và State là bắt buộc." });
                }

                // Nếu địa chỉ được đặt làm mặc định, bỏ chọn các địa chỉ mặc định khác
                if (address.IsDefault)
                {
                    var existingAddresses = await _addressService.GetAddressesByUserId(address.UserId);
                    foreach (var existingAddress in existingAddresses)
                    {
                        if (existingAddress.IsDefault)
                        {
                            existingAddress.IsDefault = false;
                            await _addressService.UpdateAddress(existingAddress);
                        }
                    }
                }

                await _addressService.AddAddress(address);
                return Ok(new { message = "Địa chỉ đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "Không có chi tiết lỗi nội bộ.";
                return BadRequest(new { message = "Lỗi khi lưu địa chỉ.", innerException = innerExceptionMessage });
            }
        }

        [HttpPut("{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] Address address)
        {
            try
            {
                if (addressId != address.AddressId)
                {
                    return BadRequest(new { message = "AddressId không khớp." });
                }
                await _addressService.UpdateAddress(address);
                return Ok(address);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                await _addressService.DeleteAddress(addressId);
                return Ok(new { message = "Đã xóa địa chỉ." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}