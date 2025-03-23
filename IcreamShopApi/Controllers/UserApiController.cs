using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authorization;
using IcreamShopApi.Repository;
using System.Data;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly UserService _userService;

        public UserApiController(UserService userService)
        {
            _userService = userService;
        }

        //get api/user
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        //get user theo id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var getUserId = await _userService.GetUserById(id);
            return Ok(getUserId);
        }

        //add user
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash); // Băm mật khẩu
            await _userService.AddUser(user);
            return user;
        }

        //delete user
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result) return NotFound("Không tìm thấy người dùng");
            return Ok("Xóa thành công");
        }

        //edit user

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> EditUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
                return BadRequest("ID không khớp");

            var existingUser = await _userService.GetUserById(id);
            if (existingUser == null)
                return NotFound("Không tìm thấy người dùng");

            // Giữ nguyên PasswordHash nếu không được gửi lên
            user.PasswordHash = user.PasswordHash ?? existingUser.PasswordHash;

            await _userService.EditUser(user);
            return Ok("Cập nhật thành công");
        }
    }
}
