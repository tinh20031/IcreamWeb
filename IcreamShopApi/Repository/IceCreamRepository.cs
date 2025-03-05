using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
	public class IceCreamRepository
	{

		private readonly CreamDbContext _context;

		public IceCreamRepository(CreamDbContext context)
		{
			_context = context;
		}
		public async Task<List<IceCream>> GetAllIceCreams()
		{
			return await _context.IceCreams.ToListAsync();
		}

		public async Task<IceCream> GetIcecreamById(int Id)
		{
			return await _context.IceCreams.FindAsync(Id);

		}

		public async Task AddIceCream(IceCream iceCream)
		{
			await _context.IceCreams.AddAsync(iceCream);
			await _context.SaveChangesAsync();

		}

		public async Task<bool> DeleteIceCream(int Id)
		{
			var icecream = await _context.IceCreams.FindAsync(Id);
			if (icecream != null)
			{
				_context.IceCreams.Remove(icecream);
				await _context.SaveChangesAsync();
				
			}
			return true;

		}

		public async Task EditIceCream(IceCream iceCream)
		{
			var existingIceCream = await _context.IceCreams.FindAsync(iceCream.IceCreamId);

			if (existingIceCream == null)
			{
				throw new Exception("Không tìm thấy Ice Cream!");
			}

			_context.Entry(existingIceCream).CurrentValues.SetValues(iceCream);
			await _context.SaveChangesAsync();
		}


	}
}




