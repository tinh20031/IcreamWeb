using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IcreamShopApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IceCreamApiController : ControllerBase
	{
		private readonly IceCreamService _iceCreamService;

		public IceCreamApiController(IceCreamService iceCreamService)
		{
			_iceCreamService = iceCreamService;
		}

		// GET: api/icecream
		[HttpGet]
		public async Task<ActionResult<List<IceCream>>> GetAllIceCreams()
		{
			var iceCreams = await _iceCreamService.GetAllIceCreams();
			return Ok(iceCreams);
		}

		[HttpGet("search")]
		//search 
		public async Task<ActionResult<List<IceCream>>> search(string name )
		{
			var search = await _iceCreamService.search(name);
			return Ok(search);
		}


		//get kem theo id 
		[HttpGet("{id}")]
		public async Task<ActionResult<IceCream>> GetIcreamById( int id)
		{
			var getIcecreamId = await _iceCreamService.GetIceCreamById(id);
			return Ok(getIcecreamId);
		}

		//theem kem 

		[HttpPost]
		public async Task<ActionResult<IceCream>> AddIceCream ([FromBody] IceCream iceCream)
		{
			var addcream = await _iceCreamService.AddIceCream(iceCream);
			return Ok(addcream);	
		}

		//xoa 
		[HttpDelete("{id}")]
		public async Task<ActionResult<IceCream>> deleteIceCream(int id)
		{
			await _iceCreamService.DeleteIceCream(id);
			return Ok();

		}
		//edit 
		[HttpPut("{id}")]
		public async Task<ActionResult<IceCream>> EditCream(	int id, [FromBody]  IceCream iceCream)
		{
			iceCream.IceCreamId = id;
			await _iceCreamService.EditIceCream(iceCream);
			return Ok();
		}

	}
}
