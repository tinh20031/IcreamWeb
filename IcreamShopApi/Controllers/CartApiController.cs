using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;

        public CartApiController(CartService cartService, OrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;

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
            cart.CartId = id;
            await _cartService.EditCart(cart);
            return Ok();
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
		public async Task<ActionResult<Order>> CreateOrderFromCart(int userId)
		{
			try
			{
				var order = await _orderService.CreateOrderFromCart(userId);
				return Ok(order);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
