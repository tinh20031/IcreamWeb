using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;

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

        public async Task<List<Cart>> GetAllCarts()
        {
            return await _cartRepository.GetAllCarts();
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
                throw new Exception("Khong tim thay Cart");
            }
            await _cartRepository.EditCart(cart);
        }
    } 
}
