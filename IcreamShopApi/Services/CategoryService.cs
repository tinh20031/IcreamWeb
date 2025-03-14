using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly CreamDbContext _context;
        public CategoryService(CategoryRepository categoryRepository, CreamDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoryRepository.GetAllCategories();
        }

        public async Task<Category> GetCategoryById(int Id)
        {
            return await _categoryRepository.GetCategoryById(Id);
        }

        public async Task<Category> AddCategory(Category category)
        {
            await _categoryRepository.AddCategory(category);
            return category;
        }

        public async Task<bool> DeleteCategory(int Id)
        {
            await _categoryRepository.DeleteCategory(Id);
            return true;
        }

        public async Task EditCategory(Category category)
        {
            var existingCategory = await _categoryRepository.GetCategoryById(category.CategoryId);
            if (existingCategory == null)
            {
                throw new Exception("Khong tim thay category!");
            }
            await _categoryRepository.EditCategory(category);
        }
		public async Task<List<IceCream>> GetProductsByCategoryId(int categoryId)
		{
			return await _context.IceCreams
				.Where(p => p.CategoryId == categoryId)
				.ToListAsync();
		}
	}
}
