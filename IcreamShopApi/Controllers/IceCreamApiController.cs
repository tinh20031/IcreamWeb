using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authorization;
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

        // GET: api/icecream/search?name={name}
        [HttpGet("search")]
        public async Task<ActionResult<List<IceCream>>> Search(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Tên không được để trống.");
            }
            var searchResult = await _iceCreamService.search(name);
            return Ok(searchResult);
        }

        // GET: api/icecream/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IceCream>> GetIceCreamById(int id)
        {
            var iceCream = await _iceCreamService.GetIceCreamById(id);
            if (iceCream == null)
            {
                return NotFound($"Không tìm thấy IceCream với ID {id}.");
            }
            return Ok(iceCream);
        }

        // POST: api/icecream
        [HttpPost]
        public async Task<ActionResult<IceCream>> AddIceCream([FromBody] IceCream iceCream)
        {
            if (iceCream == null || string.IsNullOrEmpty(iceCream.Name) || iceCream.Price <= 0 || iceCream.CategoryId <= 0)
            {
                return BadRequest("Dữ liệu IceCream không hợp lệ.");
            }

            
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var addedIceCream = await _iceCreamService.AddIceCream(iceCream);
                return CreatedAtAction(nameof(GetIceCreamById), new { id = addedIceCream.IceCreamId }, addedIceCream);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/icecream/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIceCream(int id)
        {
            var result = await _iceCreamService.DeleteIceCream(id);
            if (!result)
            {
                return NotFound($"Không tìm thấy IceCream với ID {id} để xóa.");
            }
            return NoContent();
        }

        // PUT: api/icecream/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditIceCream(int id, [FromBody] IceCream iceCream)
        {
            if (iceCream == null || id != iceCream.IceCreamId || iceCream.CategoryId <= 0)
            {
                return BadRequest("Dữ liệu IceCream không hợp lệ hoặc ID không khớp.");
            }

          
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _iceCreamService.EditIceCream(iceCream);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}