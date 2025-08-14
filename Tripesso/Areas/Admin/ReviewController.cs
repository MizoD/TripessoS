using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.UserReviewRequest;
using Models.DTOs.Response.UserReviewResponse;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("admin/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: admin/reviews
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Trip)
                .Include(r => r.Hotel)
                .ToListAsync();

            var reviewResponses = reviews.Select(r => new UserReviewResponse
            {
                Id = r.Id,
                UserId = r.UserId!,
                UserName = r.User?.UserName ?? "",
                TripId = r.TripId,
                TripName = r.Trip?.Title,
                HotelId = r.HotelId,
                HotelName = r.Hotel?.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(reviewResponses);
        }

        // ✅ POST: admin/reviews
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.TripId == null && request.HotelId == null)
                return BadRequest("Review must be linked to either a Trip or a Hotel.");

            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            var review = new Review
            {
                UserId = request.UserId,
                TripId = request.TripId,
                HotelId = request.HotelId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // رجع DTO كامل
            var response = new UserReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId!,
                UserName = (await _context.Users.FindAsync(review.UserId))?.UserName ?? "",
                TripId = review.TripId,
                TripName = (await _context.Trips.FindAsync(review.TripId))?.Title,
                HotelId = review.HotelId,
                HotelName = (await _context.Hotel.FindAsync(review.HotelId))?.Name,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            return Ok(response);
        }

        // ✅ PUT: admin/reviews/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Review not found");

            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.TripId = request.TripId;
            review.HotelId = request.HotelId;

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            var response = new UserReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId!,
                UserName = (await _context.Users.FindAsync(review.UserId))?.UserName ?? "",
                TripId = review.TripId,
                TripName = (await _context.Trips.FindAsync(review.TripId))?.Title,
                HotelId = review.HotelId,
                HotelName = (await _context.Hotel.FindAsync(review.HotelId))?.Name,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            return Ok(response);
        }

        // ✅ DELETE: admin/reviews/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound("Review not found");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review deleted successfully" });
        }
    }
}
