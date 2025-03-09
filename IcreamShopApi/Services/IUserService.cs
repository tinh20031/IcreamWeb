using IcreamShopApi.DTOs;
using IcreamShopApi.Models;
namespace IcreamShopApi.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> Register(RegisterDto model);
        Task<AuthResponseDto> Login(LoginDto model);
        string GenerateJwtToken(User user);
    }
}
