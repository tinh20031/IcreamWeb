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

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int Id)
        {
            return await _context.Categories.FindAsync(Id);
        }

        public async Task AddCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCategory(int Id)
        {
            var category = await _context.Categories.FindAsync(Id);
            if(category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                
            }
            return true;
        }

        public async Task EditCategory(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
            if(existingCategory == null)
            {
                throw new Exception("Khong tim thay Category");
            }

            _context.Entry(existingCategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
        }
        
    }
}
