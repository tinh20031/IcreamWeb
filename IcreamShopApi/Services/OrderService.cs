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
        private readonly AddressService _addressService;

        public OrderService(OrderRepository orderRepository, CreamDbContext context, CartRepository cartRepository, OrderDetailRepository orderDetailRepository, AddressService addressService)
        {
            _orderRepository = orderRepository;
            _context = context;
            _cartRepository = cartRepository;
            _orderDetailRepository = orderDetailRepository;
            _addressService = addressService;
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

        public async Task<Order> CreateOrderFromCart(int userId, string shippingAddress, int? addressId = null)
        {
            // Validate userId
            if (userId <= 0)
            {
                throw new ArgumentException("UserId không hợp lệ.");
            }

            // Validate shippingAddress
            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                throw new ArgumentException("Địa chỉ giao hàng không được để trống.");
            }

            // Get user's cart items
            var cartItems = await _cartRepository.GetCartsByUserId(userId);
            if (cartItems == null || !cartItems.Any())
            {
                throw new Exception("Giỏ hàng trống! Không thể tạo đơn hàng.");
            }

            // Validate cart data
            foreach (var item in cartItems)
            {
                if (item.IceCreamId <= 0 || item.Quantity <= 0 || item.Price <= 0)
                {
                    throw new Exception($"Dữ liệu giỏ hàng không hợp lệ (CartId: {item.CartId}).");
                }

                var iceCreamExists = await _context.IceCreams.AnyAsync(ic => ic.IceCreamId == item.IceCreamId);
                if (!iceCreamExists)
                {
                    throw new Exception($"Kem với IceCreamId {item.IceCreamId} không tồn tại.");
                }
            }

            // Validate user existence
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                throw new Exception($"Người dùng với UserId {userId} không tồn tại.");
            }

            // Calculate total price
            decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

            // Handle address (if addressId is null, save new address)
            int? newAddressId = null;
            if (addressId == null && !string.IsNullOrWhiteSpace(shippingAddress))
            {
                var addressParts = shippingAddress.Split(',').Select(p => p.Trim()).ToArray();
                if (addressParts.Length < 4)
                {
                    throw new Exception("Địa chỉ không đầy đủ. Vui lòng nhập đầy đủ số nhà, phường/xã, quận/huyện, tỉnh/thành phố.");
                }

                var newAddress = new Address
                {
                    UserId = userId,
                    Street = addressParts[0],
                    Ward = addressParts[1],
                    District = addressParts[2],
                    Province = addressParts[3],
                    City = addressParts[2], // City corresponds to District
                    State = addressParts[3], // State corresponds to Province
                    IsDefault = false
                };

                await _addressService.AddAddress(newAddress);
                newAddressId = newAddress.AddressId;
            }
            else
            {
                newAddressId = addressId;
            }

            // Create the Order
            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                ShippingAddress = shippingAddress, // Set the ShippingAddress property
                OrderDetails = cartItems.Select(cart => new OrderDetail
                {
                    IceCreamId = cart.IceCreamId,
                    Quantity = cart.Quantity,
                    Price = cart.Price
                }).ToList()
            };

            // Save the Order to the database
            await _orderRepository.AddOrder(order);

            // Clear the cart after successful order creation
            await _cartRepository.DeleteCartsByUserId(userId);

            return order;
        }


    }


}