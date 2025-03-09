using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
    public class CartRepository
    {
        private readonly CreamDbContext _context;
        public CartRepository(CreamDbContext context)
        {
            _context = context;
        }

		public async Task<List<Cart>> GetAllCarts()
		{
			return await _context.Carts
				.Include(c => c.IceCream) 
				.ToListAsync();
		}

		public async Task<Cart> GetCartById(int Id)
        {
            return await _context.Carts.FindAsync(Id);
        }

        public async Task AddCart(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCart(int Id)
        {
            var cart = await _context.Carts.FindAsync(Id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

            }
            return true;
        }

        public async Task EditCart(Cart cart)
        {
            var existingCart = await _context.Carts.FindAsync(cart.CartId);
            if (existingCart == null)
            {
                throw new Exception("Khong tim thay Cart");
            }

            _context.Entry(existingCart).CurrentValues.SetValues(cart);
            await _context.SaveChangesAsync();
        }
		public async Task<List<Cart>> GetCartsByUserId(int userId)
		{
			return await _context.Carts
				.Include(c => c.IceCream)
				.Where(c => c.UserId == userId)
				.ToListAsync();
		}
	}
}
