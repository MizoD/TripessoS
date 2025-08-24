using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.HotelRequest;
using Models.DTOs.Response.HotelResponse;
using System.Security.Claims;

namespace Tripesso.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area("Customer")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;


        public HotelsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetHotels")]
        public async Task<IActionResult> GetHotels([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 6,
            [FromQuery] string sortBy = "price",
            [FromQuery] string sortOrder = "asc")
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 6;

            var hotels = await unitOfWork.HotelRepository.GetAsync(includes: h=> h.Include(h => h.Country).Include(h => h.Trip).Include(h => h.Bookings));

            // Sorting
            hotels = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("price", "desc") => hotels.OrderByDescending(h=> h.PricePerNight),
                _ => hotels.OrderBy(x => x.PricePerNight)
            };

            // pagination 
            var totalCount = hotels.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var data = hotels.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            var result = new PaginatedHotelResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };

            return Ok(result);
        }
        [HttpGet("HotelDetails/{id}")]
        public async Task<IActionResult> GetHotelDetails(int id)
        {
            var hotel = await unitOfWork.HotelRepository.GetOneAsync(h => h.Id == id,
                includes: q => q.Include(h => h.Country).Include(h => h.Trip).Include(h => h.Bookings));

            if (hotel == null)
                return NotFound(new { message = "Hotel not found" });

            return Ok(hotel);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int hotelId, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            if (checkInDate >= checkOutDate || checkInDate < DateTime.Today)
            {
                return BadRequest("Invalid check-in/check-out dates.");
            }


            var hotel = await unitOfWork.HotelRepository.GetOneAsync(t => t.Id == hotelId);

            if (hotel == null)
                return NotFound("Hotel not found.");

            if (hotel.AvailableRooms < numberOfGuests)
                return BadRequest("Not enough available Rooms.");

            // Checks if the hotel is already in the user's cart
            var existingCartItem = await unitOfWork.HotelCartRepository.GetOneAsync(
                c => c.UserId == user.Id && c.HotelId == hotelId
            );

            if (existingCartItem != null)
            {
                existingCartItem.NumberOfPassengers++;
            }
            else
            {
                var cartItem = new HotelCart
                {
                    HotelId = hotel.Id,
                    UserId = user.Id,
                    NumberOfPassengers = numberOfGuests,
                    AddedAt = DateTime.UtcNow
                };

                var added = await unitOfWork.HotelCartRepository.CreateAsync(cartItem);
                if (!added)
                    return StatusCode(500, "Something went wrong while adding to cart.");
            }

            await unitOfWork.CommitAsync();
            return Ok("Room added to cart successfully.");
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist(int hotelId)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            var hotel = await unitOfWork.HotelRepository.GetOneAsync(t => t.Id == hotelId);

            if (hotel == null)
                return NotFound("Hotel not found.");

            // Check if it's already in wishlist
            var existingWishlistItem = await unitOfWork.HotelWishlistRepository.GetOneAsync(
                w => w.UserId == user.Id && w.HotelId == hotelId
            );

            if (existingWishlistItem != null)
                return BadRequest("This trip is already in your wishlist.");

            var wishlistItem = new HotelWishlist
            {
                HotelId = hotelId,
                UserId = user.Id,
                AddedAt = DateTime.UtcNow
            };

            var added = await unitOfWork.HotelWishlistRepository.CreateAsync(wishlistItem);
            if (!added)
                return StatusCode(500, "Something went wrong while adding to wishlist.");

            return Ok("Hotel added to wishlist successfully.");
        }

        //[HttpPost("Review")]
        //[Authorize]
        //public async Task<IActionResult> AddHotelReview([FromBody] AddHotelReviewRequest request)
        //{
        //    var user = await unitOfWork.UserManager.GetUserAsync(User);
        //    if (user is null)
        //    {
        //        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        //        user = await unitOfWork.UserManager.FindByIdAsync(userId);
        //    }
        //    if (user == null)
        //        return Unauthorized();

        //    var hadBooked = await unitOfWork.BookingRepository.GetOneAsync(b => b.UserId == user.Id && b.HotelId == request.HotelId);

        //    if (hadBooked is null)
        //        return Forbid("You can only review hotels you have booked.");

        //    bool alreadyReviewed = await unitOfWork.ReviewRepository
        //        .GetOneAsync(r => r.HotelId == request.HotelId && r.UserId == userId);

        //    if (alreadyReviewed)
        //        return BadRequest("You have already reviewed this hotel.");

        //    // 4. Create review
        //    var review = new Review
        //    {
        //        HotelId = request.HotelId,
        //        UserId = userId,
        //        Rating = request.Rating,
        //        Comment = request.Comment,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _reviewRepository.CreateAsync(review);
        //    await _reviewRepository.CommitAsync();

        //    // 5. Response (dynamic TripId / HotelId)
        //    var response = new HotelReviewResponse
        //    {
        //        Id = review.Id,
        //        HotelId = review.HotelId,
        //        UserName = user.UserName,
        //        Rating = review.Rating,
        //        Comment = review.Comment,
        //        CreatedAt = review.CreatedAt
        //    };

        //    return Ok(response);
        //}

        [HttpPost("SearchHotels")]
        public async Task<IActionResult> SearchHotels(
    [FromBody] HotelSearchRequest request,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 6,
    [FromQuery] string sortBy = "price",
    [FromQuery] string sortOrder = "asc")
        {
            var hotels = await unitOfWork.HotelRepository.GetAsync(h=> 
                    h.AvailableRooms >= request.NumberOfGuests &&
                    h.Country.Name.ToLower().Contains(request.Country?? "".ToLower()),
                includes: h => h
                    .Include(h => h.Country).Include(h => h.Trip).Include(h=> h.Bookings)
            );

            // sort
            hotels = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc"
                    ? hotels.OrderByDescending(r => r.PricePerNight)
                    : hotels.OrderBy(r => r.PricePerNight),
                _ => hotels.OrderBy(r => r.PricePerNight)
            };

            // Pagination
            var totalCount = hotels.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginatedData = hotels
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new HotelSerachResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = paginatedData
            };

            return Ok(result);
        }

    }
}
