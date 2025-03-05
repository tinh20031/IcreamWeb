using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
	public class CategoryRepository
	{

		private readonly CreamDbContext _context;

		public CategoryRepository(CreamDbContext context)
		{
			_context = context;
		}

		public async Task<List<Category>> Listcategory()
		{
			return await _context.Categories.ToListAsync();
		}






	}
}
