using IcreamShopApi.Data;
using IcreamShopApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IcreamShopApi.Repository
{
    public class ReviewRepository
    {
        private CreamDbContext _context;

        public ReviewRepository(CreamDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task AddReview(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review> GetReviewById(int Id)
        {
            return await _context.Reviews.FindAsync(Id);
        }

        public async Task<bool> DeleteReview(int Id)
        {
            var review = await _context.Reviews.FindAsync(Id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task EditReview(Review review)
        {
            var existingReview = await _context.Reviews.FindAsync(review.ReviewId);
            if (existingReview == null)
            {
                throw new Exception("Khong tim thay Review");
            }

            _context.Entry(existingReview).CurrentValues.SetValues(review);
            await _context.SaveChangesAsync();
        }
    }
}
