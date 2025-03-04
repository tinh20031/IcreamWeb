using IcreamShopApi.Models;
using IcreamShopApi.Repository;

namespace IcreamShopApi.Services
{
	public class IceCreamService
	{
		private readonly IceCreamRepository _iceCreamRepository;
		public IceCreamService(IceCreamRepository iceCreamRepository)
		{
			_iceCreamRepository = iceCreamRepository;
		}
		public async Task<List<IceCream>> GetAllIceCreams()
		{
			return await _iceCreamRepository.GetAllIceCreams();
		}

		public async Task<IceCream> GetIceCreamById (int id)
		{
			return await _iceCreamRepository.GetIcecreamById(id);
		}

		public async Task AddIceCream (IceCream iceCream)
		{
			 await _iceCreamRepository.AddIceCream(iceCream);
		}

		public async Task DeleteIceCream (int id)
		{
			await _iceCreamRepository.DeleteIceCream(id);	
		}
	}
}
