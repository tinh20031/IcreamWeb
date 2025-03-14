using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryApiController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //Get: api/category
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }

        //get category theo id
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var getCategoryId = await _categoryService.GetCategoryById(id);
            return Ok(getCategoryId);
        }

        //add category
        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory([FromBody] Category category)
        {
            var addcategory = await _categoryService.AddCategory(category);
            return Ok(addcategory);
        }

        //delete category
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategory(id);
            return Ok();
        }

        [HttpPut("{id}")]
        //edit category
        public async Task<ActionResult<Category>> EditCategory(int id, [FromBody] Category category)
        {
            category.CategoryId = id;
            await _categoryService.EditCategory(category);
            return Ok();
        }
		[HttpGet("{id}/ice_cream")]
		public async Task<ActionResult<List<IceCream>>> GetProductsByCategoryId(int id)
		{
			var products = await _categoryService.GetProductsByCategoryId(id);
			if (products == null || !products.Any()) return NotFound("Không tìm thấy sản phẩm cho danh mục này.");
			return Ok(products);
		}
	}
}
 