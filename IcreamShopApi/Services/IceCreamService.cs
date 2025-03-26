using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;

namespace IcreamShopApi.Services
{
	public class IceCreamService
	{
		private readonly IceCreamRepository _iceCreamRepository;
		public readonly CreamDbContext _context; 


		public IceCreamService(IceCreamRepository iceCreamRepository, CreamDbContext context)
		{
			_iceCreamRepository = iceCreamRepository;
			_context = context;
		}
		public async Task<List<IceCream>> GetAllIceCreams()
		{
			return await _iceCreamRepository.GetAllIceCreams();
		}

		public async Task<List<IceCream>> search(string name ) { 
		
		return await _iceCreamRepository.Search( name );
		
		
		
		}





		public async Task<IceCream> GetIceCreamById(int id)
		{
			return await _iceCreamRepository.GetIcecreamById(id);
		}

		public async Task<IceCream> AddIceCream(IceCream iceCream)
		{
			var findIdCategory = await _context.Categories.FindAsync(iceCream.CategoryId);
			await _iceCreamRepository.AddIceCream(iceCream);

			//trả về kem mới thêm 
			return iceCream;
		}

		public async Task<bool> DeleteIceCream(int id)
		{
			await _iceCreamRepository.DeleteIceCream(id);
			return true;
		}

		public async Task EditIceCream(IceCream iceCream)
		{
			var existingIceCream = await _iceCreamRepository.GetIcecreamById(iceCream.IceCreamId);

			if (existingIceCream == null)
			{
				throw new Exception("Không tìm thấy Ice Cream!");
			}

			// Gọi trực tiếp repository để cập nhật
			await _iceCreamRepository.EditIceCream(iceCream);
		}
	}
	}
