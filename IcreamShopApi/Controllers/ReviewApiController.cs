using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewApiController : Controller
    {
        private readonly ReviewService _reviewService;
        private readonly CreamDbContext _context;

        public ReviewApiController(ReviewService reviewService, CreamDbContext context)
        {
            _reviewService = reviewService;
            _context = context;
        }

        //Get: api/review

        /*[HttpGet]
        public async Task<ActionResult<List<Review>>> GetAllReviews([FromQuery] int? iceCreamId = null)
        {
            var reviews = await _reviewService.GetAllReviews();
            if (iceCreamId.HasValue)
            {
                reviews = reviews.Where(r => r.IceCreamId == iceCreamId.Value).ToList();
            }
            return Ok(reviews);
        }*/
        // GET: api/review
        [HttpGet]
        public async Task<ActionResult<List<ReviewDto>>> GetAllReviews([FromQuery] int? iceCreamId = null)
        {
            // Truy vấn trực tiếp từ DbContext
            var query = _context.Reviews.AsQueryable();
            if (iceCreamId.HasValue)
            {
                query = query.Where(r => r.IceCreamId == iceCreamId.Value);
            }

            // Chuyển đổi sang ReviewDto và lấy FullName từ bảng User
            var reviewDtos = await query.Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                FullName = _context.Users
                    .Where(u => u.UserId == r.UserId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? "Unknown",
                IceCreamId = r.IceCreamId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToListAsync();

            return Ok(reviewDtos);
        }

        //get review theo id
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReviewById(int id)
        {
            var getReviewId = await _reviewService.GetReviewById(id);
            return Ok(getReviewId);
        }

        //add review
        [HttpPost]
        public async Task<ActionResult<Review>> AddReview([FromBody] Review review)
        {
            var addReview = await _reviewService.AddReview(review);
            return Ok(addReview);
        }

        //delete review
        [HttpDelete("{id}")]
        public async Task<ActionResult<Review>> DeleteReview(int id)
        {
            await _reviewService.DeleteReview(id);
            return Ok();
        }

        [HttpPut("{id}")]
        //edit review
        public async Task<ActionResult<Review>> EditReview(int id, [FromBody] Review review)
        {
            review.ReviewId = id;
            await _reviewService.EditReview(review);
            return Ok();
        }
    }
}
