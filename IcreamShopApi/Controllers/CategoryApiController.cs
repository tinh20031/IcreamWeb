using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;

        public CategoryApiController(CategoryService categoryService, IWebHostEnvironment environment)
        {
            _categoryService = categoryService;
            _environment = environment;
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
        //[Authorize(Roles = "Admin")]
        /*[HttpPost]
        public async Task<ActionResult<Category>> AddCategory([FromBody] Category category)
        {
            var addcategory = await _categoryService.AddCategory(category);
            return Ok(addcategory);
        }*/
        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory([FromForm] string name, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Ảnh là bắt buộc");

            if (string.IsNullOrEmpty(_environment.WebRootPath))
                return StatusCode(500, "Web root path is not configured");

            // Lưu file vào thư mục wwwroot/uploads
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Đảm bảo tên file có phần mở rộng
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var category = new Category
            {
                Name = name,
                image = $"/uploads/{fileName}" // Đảm bảo đường dẫn bắt đầu bằng /uploads/
            };

            var addedCategory = await _categoryService.AddCategory(category);
            return Ok(addedCategory);
        }

        //delete category
        //[Authorize(Roles = "Admin")]
        /*[HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategory(id);
            return Ok();
        }*/
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
                return NotFound("Không tìm thấy danh mục");

            // Xóa ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(category.image) && System.IO.File.Exists(Path.Combine(_environment.WebRootPath, category.image.TrimStart('/'))))
            {
                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, category.image.TrimStart('/')));
            }

            await _categoryService.DeleteCategory(id);
            return Ok();
        }

        //[Authorize(Roles = "Admin")]
        /*[HttpPut("{id}")]
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
		}*/
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> EditCategory(int id, [FromForm] string name, IFormFile? image)
        {
            var existingCategory = await _categoryService.GetCategoryById(id);
            if (existingCategory == null)
                return NotFound("Không tìm thấy danh mục");

            if (string.IsNullOrEmpty(_environment.WebRootPath))
                return StatusCode(500, "Web root path is not configured");

            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingCategory.image) && System.IO.File.Exists(Path.Combine(_environment.WebRootPath, existingCategory.image.TrimStart('/'))))
                {
                    System.IO.File.Delete(Path.Combine(_environment.WebRootPath, existingCategory.image.TrimStart('/')));
                }

                existingCategory.image = $"/uploads/{fileName}";
            }

            existingCategory.Name = name;
            await _categoryService.EditCategory(existingCategory);
            return Ok();
        }
    }
}
 