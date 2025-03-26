using IcreamShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            var addUser = await _userService.AddUser(user);
            return Ok(addUser);
        }

        //delete user
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }

        //edit user
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> EditUser(int id, [FromBody] User user)
        {
            user.UserId = id;
            await _userService.EditUser(user);
            return Ok();
        }
    }
}
