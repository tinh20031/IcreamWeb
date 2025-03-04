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


	}
}
