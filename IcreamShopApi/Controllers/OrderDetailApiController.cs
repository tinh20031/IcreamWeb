using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailApiController : Controller
    {
       private readonly OrderDetailService _orderDetailService;

       public OrderDetailApiController(OrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDetail>>> GetAllOrderDetails()
        {
            var orderDetails = await _orderDetailService.GetAllOrderDetails();
            return Ok(orderDetails);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetailById(int id)
        {
            var getOrderDetailId = await _orderDetailService.GetOrderDetailById(id);
            return Ok(getOrderDetailId);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetail>> AddOrderDetail([FromBody] OrderDetail orderDetail)
        {
            var addOrderDetail = await _orderDetailService.AddOrderDetail(orderDetail);
            return Ok(addOrderDetail);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderDetail>> DeleteOrderDetail(int id)
        {
            await _orderDetailService.DeleteOrderDetail(id);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDetail>> EditOrderDetail(int id, [FromBody] OrderDetail orderDetail)
        {
            orderDetail.OrderDetailId = id;
            await _orderDetailService.EditOrderDetail(orderDetail);
            return Ok();
        }
    }
}
