using IcreamShopApi.Data;
using IcreamShopApi.DTOs;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepository;
        private readonly CreamDbContext _context;
        public CartService(CartRepository cartRepository, CreamDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

		public async Task<List<CartDTO>> GetAllCarts()
		{
			var carts = await _context.Carts
				.Include(c => c.IceCream) 
				.Select(c => new CartDTO
				{
					CartId = c.CartId,
					UserId = c.UserId,
					IceCreamId = c.IceCreamId,
					Quantity = c.Quantity,
					CreatedAt = c.CreatedAt,
					IceCreamName = c.IceCream.Name, 
					Image = c.IceCream.ImageUrl,
                    Price = c.IceCream.Price,
				})
				.ToListAsync();

			return carts;
		}

		public async Task<Cart> GetCartById(int Id)
        {
            return await _cartRepository.GetCartById(Id);
        }

        public async Task<Cart> AddCart(Cart cart)
        {
            await _cartRepository.AddCart(cart);
            return cart;
        }

        public async Task<bool> DeleteCart(int Id)
        {
            await _cartRepository.DeleteCart(Id);
            return true;
        }

        public async Task EditCart(Cart cart)
        {
            var existingCart = await _cartRepository.GetCartById(cart.CartId);
            if (existingCart == null)
            {
                throw new Exception($"Không tìm thấy giỏ hàng với ID {cart.CartId}.");
            }

            // Cập nhật các thuộc tính
            existingCart.UserId = cart.UserId;
            existingCart.IceCreamId = cart.IceCreamId;
            existingCart.Quantity = cart.Quantity;
            existingCart.CreatedAt = cart.CreatedAt;

            await _cartRepository.EditCart(existingCart);
        }


        public async Task<List<CartDTO>> GetCartsByUserIdAsync(int userId)
        {
            var carts = await _context.Carts
                .Include(c => c.IceCream)
                .Where(c => c.UserId == userId)
                .Select(c => new CartDTO
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    IceCreamId = c.IceCreamId,
                    Quantity = c.Quantity,
                    CreatedAt = c.CreatedAt,
                    IceCreamName = c.IceCream.Name,
                    Image = c.IceCream.ImageUrl,
                    Price = c.IceCream.Price
                })
                .ToListAsync();

            if (carts == null || !carts.Any())
            {
                throw new Exception($"Không tìm thấy giỏ hàng cho người dùng với ID {userId}");
            }

            return carts;
        }
    } 
}
