using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderApiController : Controller
    {
        private readonly OrderService _orderService;

        public OrderApiController(OrderService orderService)
        {
            _orderService = orderService;
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
    }
}
