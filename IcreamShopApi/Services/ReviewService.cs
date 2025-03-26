using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;

namespace IcreamShopApi.Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _reviewRepository;
        private readonly CreamDbContext _context;

        public ReviewService(ReviewRepository reviewRepository, CreamDbContext context)
        {
            _reviewRepository = reviewRepository;
            _context = context;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            return await _reviewRepository.GetAllReviews();
        }

        public async Task<Review> GetReviewById(int Id)
        {
            return await _reviewRepository.GetReviewById(Id);
        }

        public async Task<Review> AddReview(Review review)
        {
            await _reviewRepository.AddReview(review);
            return review;
        }

        public async Task<bool> DeleteReview(int Id)
        {
            await _reviewRepository.DeleteReview(Id);
            return true;
        }

        public async Task EditReview(Review review)
        {
            var existingReview = await _reviewRepository.GetReviewById(review.ReviewId);
            if (existingReview == null)
            {
                throw new Exception("Khong tim thay Review");
            }

            await _reviewRepository.EditReview(review);
        }


    }
}
