using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
	public class IceCreamRepository
	{

		private readonly AppDbContext _context;

		public IceCreamRepository(AppDbContext context)
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

		public async Task DeleteIceCream(int Id)
		{
			var icecream = await _context.IceCreams.FindAsync(Id);
			if (icecream != null)
			{
				_context.IceCreams.Remove(icecream);
				await _context.SaveChangesAsync();

			}

		}

		public async Task EditIceCream (int id)
		{
			var editIceCream = await _context.IceCreams.FindAsync($"{id}");
		}
		
	}
}




