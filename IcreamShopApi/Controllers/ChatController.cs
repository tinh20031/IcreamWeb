using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using IcecreamShopApi.Services;

namespace IcecreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Text))
                return BadRequest("Tin nhắn không hợp lệ!");

            // 🔥 Phát tin nhắn qua SignalR cho tất cả client
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Text);

            return Ok(new { status = "Message sent!" });
        }
    }

    public class ChatMessage
    {
        public string User { get; set; }
        public string Text { get; set; }
    }
}
