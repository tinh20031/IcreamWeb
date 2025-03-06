using IcreamShopApi.Models;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IcreamShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewApiController : Controller
    {
        private readonly ReviewService _reviewService;

        public ReviewApiController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        //Get: api/review
        [HttpGet]
        public async Task<ActionResult<List<Review>>> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviews();
            return Ok(reviews);
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
