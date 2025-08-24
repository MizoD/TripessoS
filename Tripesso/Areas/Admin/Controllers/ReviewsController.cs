using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.UserReviewRequest;
using Models.DTOs.Response.UserReviewResponse;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ReviewsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await unitOfWork.ReviewRepository.GetAsync(includes: r=> r.Include(r=> r.User).Include(r=> r.Trip));

            var reviewResponses = reviews.Select(r => new UserReviewResponse
            {
                Id = r.Id,
                UserId = r.UserId!,
                UserName = r.User?.UserName ?? "",
                TripId = r.TripId,
                TripName = r.Trip?.Title ?? " ",
                Rating = r.Rating,
                Comment = r.Comment ?? " ",
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(reviewResponses);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserReviewRequest request)
        {
            if (request.TripId <= 0)
                return BadRequest("Review must be linked to either a Trip.");

            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            var review = new Review
            {
                UserId = request.UserId,
                TripId = request.TripId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var created = await unitOfWork.ReviewRepository.CreateAsync(review);
            if (!created) return StatusCode(500, "An error occurred while creating the review.");

            var response = new UserReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId!,
                UserName = (await unitOfWork.UserManager.FindByIdAsync(review.UserId))?.UserName ?? "",
                TripId = review.TripId,
                TripName = (await unitOfWork.TripRepository.GetOneAsync(t=> t.Id == review.TripId))?.Title ?? " ",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            return Ok(new {Message = "Review Created Successfully!", response });
        }

        
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] CreateUserReviewRequest request)
        {
            var review = await unitOfWork.ReviewRepository.GetOneAsync(r=> r.Id == request.Id);
            if (review == null) return NotFound("Review not found");

            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.TripId = request.TripId;
            review.UserId = request.UserId;

            var updated = await unitOfWork.ReviewRepository.UpdateAsync(review);
            if (!updated) return StatusCode(500, "An error occurred while updating the review.");

            var response = new UserReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId!,
                UserName = (await unitOfWork.UserManager.FindByIdAsync(review.UserId))?.UserName ?? "",
                TripId = review.TripId,
                TripName = (await unitOfWork.TripRepository.GetOneAsync(t => t.Id == review.TripId))?.Title ?? " ",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            return Ok(new {Message = "Review Updated Successfully"});
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await unitOfWork.ReviewRepository.GetOneAsync(t => t.Id == id);
            if (review == null) return NotFound("Review not found");

            var deleted = await unitOfWork.ReviewRepository.DeleteAsync(review);
            if (!deleted) return StatusCode(500, "An error occurred while deleting the review.");

            return Ok(new { message = "Review deleted successfully" });
        }
    }
}
