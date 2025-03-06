using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;

namespace IcreamShopApi.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly CreamDbContext _context;
        public OrderService(OrderRepository orderRepository, CreamDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _orderRepository.GetAllOrders();
        }

        public async Task<Order> GetOrderById(int Id)
        {
            return await _orderRepository.GetOrderById(Id);
        }

        public async Task<Order> AddOrder(Order order)
        {
            await _orderRepository.AddOrder(order);
            return order;
        }

        public async Task<bool> DeleteOrder(int Id)
        {
            await _orderRepository.DeleteOrder(Id);
            return true;
        }

        public async Task EditOrder(Order order)
        {
            var existingOrder = await _orderRepository.GetOrderById(order.OrderId);
            if (existingOrder == null)
            {
                throw new Exception("Khong tim thay Order");
            }
            await _orderRepository.EditOrder(order);
        }
    }
}
