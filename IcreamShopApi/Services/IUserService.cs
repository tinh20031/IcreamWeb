using IcreamShopApi.DTOs;
namespace IcreamShopApi.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> Register(RegisterDto model);
        Task<AuthResponseDto> Login(LoginDto model);
    }
}
