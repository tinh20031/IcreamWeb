using IcreamShopApi.Data;
using IcreamShopApi.DTOs;
using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderApiController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CreamDbContext _context;

        public OrderApiController(OrderService orderService, CreamDbContext context)
        {
            _orderService = orderService;
            _context = context;

		}

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var getOrderId = await _orderService.GetOrderById(id);
            return Ok(getOrderId);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder([FromBody] Order order)
        {
            var addOrder = await _orderService.AddOrder(order);
            return Ok(addOrder);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            await _orderService.DeleteOrder(id);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> EditOrder(int id, [FromBody] Order order)
        {
            order.OrderId = id;
            await _orderService.EditOrder(order);
            return Ok();
        }


		[HttpGet("user/{userId}")]
		public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUser(int userId)
		{
			var orders = await _context.Orders
				.Where(o => o.UserId == userId)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.IceCream) // Lấy thông tin kem
				.ToListAsync();

			if (orders == null || orders.Count == 0)
				return NotFound("No orders found for this user.");

			var orderDtos = orders.Select(o => new OrderDTO
			{
				OrderId = o.OrderId,
				TotalPrice = o.TotalPrice,
				Status = o.Status,
				OrderDate = o.OrderDate,
				OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
				{
					IceCreamName = od.IceCream.Name,
					ImageUrl = od.IceCream.ImageUrl,
					Quantity = od.Quantity,
					Price = od.Price
				}).ToList()
			}).ToList();

			return Ok(orderDtos);
		}
	}
}
