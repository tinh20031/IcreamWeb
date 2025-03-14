using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Services
{
	public class OrderService
	{
		private readonly OrderRepository _orderRepository;
		private readonly CreamDbContext _context;
		private readonly CartRepository _cartRepository; 
		private readonly OrderDetailRepository _orderDetailRepository; 

		public OrderService(OrderRepository orderRepository, CreamDbContext context,
						   CartRepository cartRepository, OrderDetailRepository orderDetailRepository)
		{
			_orderRepository = orderRepository;
			_context = context;
			_cartRepository = cartRepository; 
			_orderDetailRepository = orderDetailRepository; 
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

		public async Task<Order> CreateOrderFromCart(int userId)
		{
			// Lấy giỏ hàng của user
			var cartItems = await _cartRepository.GetCartsByUserId(userId);
			if (cartItems == null || !cartItems.Any())
			{
				throw new Exception("Giỏ hàng trống!");
			}

			// Tính tổng giá
			decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

			// Tạo Order
			var order = new Order
			{
				UserId = userId,
				TotalPrice = totalPrice,
				Status = "Pending",
				OrderDate = DateTime.Now
			};

			// Lưu Order vào DB và lấy bản ghi đã lưu
			await _orderRepository.AddOrder(order); // Lưu trước
			var createdOrder = await _context.Orders
				.OrderByDescending(o => o.OrderId)
				.FirstOrDefaultAsync(o => o.UserId == userId && o.OrderDate == order.OrderDate);
			if (createdOrder == null)
			{
				throw new Exception("Lỗi khi tạo đơn hàng!");
			}

			// Tạo OrderDetail từ Cart
			var orderDetails = cartItems.Select(cart => new OrderDetail
			{
				OrderId = createdOrder.OrderId,
				IceCreamId = cart.IceCreamId,
				Quantity = cart.Quantity,
				Price = cart.Price
			}).ToList();

			// Lưu OrderDetail vào DB
			foreach (var detail in orderDetails)
			{
				await _orderDetailRepository.AddOrderDetail(detail);
			}

			// Xóa giỏ hàng sau khi lưu thành công
			await _cartRepository.DeleteCartsByUserId(userId);

			return createdOrder;
		}
	}
}